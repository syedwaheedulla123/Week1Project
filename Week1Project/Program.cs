using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;



class BankAccount
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; }
    public List<string> TransactionHistory { get; set; } = new List<string>();



    public BankAccount() { }



    public BankAccount(string accountNumber, string ownerName, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = initialBalance >= 0 ? initialBalance : throw new ArgumentException("Initial balance cannot be negative.");
        TransactionHistory.Add($"Account created with initial balance: {initialBalance:C}");
    }





    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.");
        Balance += amount;
        TransactionHistory.Add($"Deposited: {amount:C}, New Balance: {Balance:C}");
        Console.WriteLine($"Deposited {amount:C}. New Balance: {Balance:C}");
    }



    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.");
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient balance.");
        Balance -= amount;
        TransactionHistory.Add($"Withdrew: {amount:C}, New Balance: {Balance:C}");
        Console.WriteLine($"Withdrew {amount:C}. New Balance: {Balance:C}");
    }



    public override string ToString() => $"Account: {AccountNumber}, Owner: {OwnerName}, Balance: {Balance:C}";
}



class Program
{
    static Dictionary<string, BankAccount> accounts = new Dictionary<string, BankAccount>();
    const string filePath = "accounts.json";



    static async Task SaveAccountsToFileAsync()
    {
        string json = JsonSerializer.Serialize(accounts);
        await File.WriteAllTextAsync(filePath, json);
    }



    static async Task LoadAccountsFromFileAsync()
    {
        if (File.Exists(filePath))
        {
            string json = await File.ReadAllTextAsync(filePath);
            accounts = JsonSerializer.Deserialize<Dictionary<string, BankAccount>>(json) ?? new Dictionary<string, BankAccount>();
        }
    }



    static void DisplayMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1. Create Account");
        Console.WriteLine("2. Deposit");
        Console.WriteLine("3. Withdraw");
        Console.WriteLine("4. View All Accounts");
        Console.WriteLine("5. Delete Account");
        Console.WriteLine("6. Bank Balance");
        Console.WriteLine("7. Exit");
        Console.Write("Choose an option: ");
    }



    static async Task Main(string[] args)
    {
        await LoadAccountsFromFileAsync();



        while (true)
        {
            DisplayMenu();
            string choice = Console.ReadLine();



            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Account Number: ");
                        string accNo = Console.ReadLine();
                        Console.Write("Enter Owner Name: ");
                        string owner = Console.ReadLine();
                        Console.Write("Enter Initial Balance: ");
                        decimal balance = decimal.Parse(Console.ReadLine());
                        accounts.Add(accNo, new BankAccount(accNo, owner, balance));
                        await SaveAccountsToFileAsync();
                        Console.WriteLine("Account created successfully.");
                        break;



                    case "2":
                        Console.Write("Enter Account Number: ");
                        accNo = Console.ReadLine();
                        if (accounts.TryGetValue(accNo, out var account))
                        {
                            Console.Write("Enter Deposit Amount: ");
                            decimal deposit = decimal.Parse(Console.ReadLine());
                            account.Deposit(deposit);
                            await SaveAccountsToFileAsync();
                        }
                        else
                        {
                            Console.WriteLine("Account not found.");
                        }
                        break;



                    case "3":
                        Console.Write("Enter Account Number: ");
                        accNo = Console.ReadLine();
                        if (accounts.TryGetValue(accNo, out account))
                        {
                            Console.Write("Enter Withdrawal Amount: ");
                            decimal withdrawal = decimal.Parse(Console.ReadLine());
                            account.Withdraw(withdrawal);
                            await SaveAccountsToFileAsync();
                        }
                        else
                        {
                            Console.WriteLine("Account not found.");
                        }
                        break;



                    case "4":
                        foreach (var acc in accounts.Values)
                        {
                            Console.WriteLine(acc);
                        }
                        break;



                    case "5":
                        Console.Write("Enter Account Number to delete: ");
                        accNo = Console.ReadLine();
                        if (accounts.Remove(accNo))
                        {
                            await SaveAccountsToFileAsync();
                            Console.WriteLine("Account deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Account not found.");
                        }
                        break;



                    case "6":
                        Console.Write("Enter Account Number: ");
                        accNo = Console.ReadLine();
                        if (accounts.TryGetValue(accNo, out account))
                        {
                            Console.WriteLine($"Account Balance for {account.OwnerName} ({account.AccountNumber}): {account.Balance:C}");
                        }
                        else
                        {
                            Console.WriteLine("Account not found.");
                        }
                        break;





                    case "7":
                        await SaveAccountsToFileAsync();
                        return;



                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
