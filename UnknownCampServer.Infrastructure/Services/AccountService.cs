using UnknownCampServer.Core.Entities;
using UnknownCampServer.Core.Repositories;
using UnknownCampServer.Core.Services;
using System.Security.Cryptography;
using UnknownCampServer.Core.DTOs;
using UnknownCampServer.Core.Models;
using MongoDB.Driver;

namespace UnknownCampServer.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        private readonly ITreasureService _treasureService;


        public AccountService(IAccountRepository accountRepository, IEmailService emailService, IPasswordService passwordService, ITreasureService treasureService)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _passwordService = passwordService;
            _treasureService = treasureService;
        }

        public async Task<Account> GetAccount(string id)
        {
            var accounts = await _accountRepository.GetAccountByIdAsync(id);
            return accounts;
        }

        public async Task<List<string>> GetAllEmailsAsync()
        {
            var accounts = await _accountRepository.GetAllVerifiedAccountsAsync();
            var emails = accounts.Select(account => account.Email).ToList();
            return emails;
        }

        public async Task<bool> CreateAccountAsync(AccountRegDTO accountDTO)
        {
            var existingAccount = await _accountRepository.GetAccountByEmailOrNameAsync(accountDTO.Email, accountDTO.Name);
            if (existingAccount != null)
            {
                throw new Exception("Account with this email or username already exists.");
            }

            var salt = _passwordService.GenerateSalt();
            string hashedPassword = _passwordService.HashPasswordPBKDF2(accountDTO.Password, salt);

            var account = new Account
            {
                Name = accountDTO.Name,
                Email = accountDTO.Email,
                PasswordHash = hashedPassword,
                DateCreation = DateTime.UtcNow,
                VerifiedAt = null,
                Token = Guid.NewGuid().ToString(),
                Unlockables = new List<AccountUnlockable>(),
                Currency = new Currency(),
                TreasureOpenings = new List<TreasureOpening>()
            };

            bool emailSent = await SendVerificationEmailAsync(account.Email, account.Token);
            if (!emailSent)
            {
                throw new Exception("Failed to send verification email.");
            }

            try
            {
                await _accountRepository.CreateAccountAsync(account);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new Exception("Account with this email or username already exists.");
            }

            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var account = await _accountRepository.GetAccountByTokenAsync(token);
            if (account == null)
                return false;

            account.VerifiedAt = DateTime.UtcNow;
            return await _accountRepository.UpdateAccountAsync(account);
        }

        private async Task<bool> SendVerificationEmailAsync(string email, string token)
        {
            var subject = "Email Verification";
            var body = $"<b>Please verify your email!<b> \n <a href='http://localhost:5000/Verification/verify-email?token={token}'>[Verify Now!]</a>";

            var emailRequest = new EmailRequest
            {
                To = email,
                Subject = subject,
                Body = body
            };

            return await _emailService.SendEmailAsync(emailRequest);
        }

        public async Task<Account> LoginAsync(AccountLoginDTO loginDto)
        {
            var account = await _accountRepository.GetAccountByEmailOrNameAsync(loginDto.Email, loginDto.Email);
            if (account == null)
            {
                throw new Exception("Invalid email or password.");
            }

            if (account.VerifiedAt == null)
            {
                throw new Exception("Email is not verified.");
            }

            if (!_passwordService.VerifyPassword(loginDto.Password, account.PasswordHash))
            {
                throw new Exception("Invalid email or password.");
            }

            return account;
        }


        public async Task<bool> AddUnlockableAsync(string accountId, string unlockableId)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null) return false;

            var newUnlockable = new AccountUnlockable
            {
                UnlockableId = unlockableId,
                DateObtained = DateTime.UtcNow
            };

            account.Unlockables.Add(newUnlockable);
            await _accountRepository.UpdateAccountAsync(account);

            return true;
        }

        public async Task<bool> BuyMatchBoxAsync(string accountId)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null) throw new Exception("Account not found.");

            if (account.Currency.Matches < 45)
                throw new Exception("NoMatches");

            account.Currency.Matches -= 45;
            account.Currency.Boxes += 1;

            await _accountRepository.UpdateAccountAsync(account);

            return true;
        }

        public async Task<TreasureResult> OpenTreasureAsync(string accountId, string treasureId)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null) throw new Exception("Account not found.");

            if (account.Currency.Boxes < 1)
                throw new Exception("NoTreasure");

            account.Currency.Boxes -= 1;

            if (account.TreasureOpenings == null)
            {
                account.TreasureOpenings = new List<TreasureOpening>();
            }

            var treasureOpening = account.TreasureOpenings
                .FirstOrDefault(to => to.TreasureID == treasureId);

            if (treasureOpening == null)
            {
                treasureOpening = new TreasureOpening
                {
                    TreasureID = treasureId,
                    Opened = 0,
                    LastOpenings = new List<TreasureOpening.LastOpening>()
                };
                account.TreasureOpenings.Add(treasureOpening);
            }

            treasureOpening.Opened += 1;

            var treasureResult = await _treasureService.UnlockTreasureAsync(treasureId, treasureOpening);

            var lastOpening = treasureOpening.LastOpenings
                .FirstOrDefault(lo => lo.Type == treasureResult.Type);

            if (lastOpening != null)
            {
                lastOpening.LastAt = treasureOpening.Opened;
            }
            else
            {
                treasureOpening.LastOpenings.Add(new TreasureOpening.LastOpening
                {
                    Type = treasureResult.Type,
                    LastAt = treasureOpening.Opened
                });
            }

            bool exists = account.Unlockables.Exists(x => x.UnlockableId == treasureResult.UnlockableId);

            if (exists)
            {
                account.Currency.Ashes += 10;
                treasureResult.Ashes = 10;
            }
            else
            {
                var newUnlockable = new AccountUnlockable
                {
                    UnlockableId = treasureResult.UnlockableId,
                    DateObtained = DateTime.UtcNow
                };

                account.Unlockables.Add(newUnlockable);
            }

            await _accountRepository.UpdateAccountAsync(account);

            return treasureResult;
        }

        public async Task<bool> AddMatchesAsync(string accountId, int matches)
        {
            if (matches > 200)
            {
                throw new ArgumentException("TooManyMatches");
            }

            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null) throw new Exception("Account not found.");

            account.Currency.Matches += matches;

            await _accountRepository.UpdateAccountAsync(account);

            return true;
        }

    }
}
