using System;
using System.Text.RegularExpressions;
using Npgsql;
using System.Windows;
using System.Security.Cryptography;
using System.Windows.Controls;
using NoteApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace NoteAppWPF;

/// <summary>
/// Interaction logic for Window1.xaml
/// </summary>
public partial class Window1 : Window
{
    private readonly Cache _cache;

    public Window1(Cache cache)
    {
        _cache = cache;
    }
    public static bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
        
    public Window1()
    {
        InitializeComponent();

        _cache = new Cache(new Context(), new MemoryCache(new MemoryCacheOptions()));
    }

    private void LogIn_Click(object sender, RoutedEventArgs e)
    {
        using (var context = new Context())
        {
            string inputLog = txtLog.Text;
            string inputPass = txtPass.Text;

            if (!(string.IsNullOrEmpty(inputLog) && string.IsNullOrEmpty(inputPass)))
            {
                    
                var authentication = new Authentication(new Context());

                bool isPasswordCorrect = authentication.AuthenticateUser(inputLog,inputPass);
                var user = context.Users
                    .FirstOrDefault(u => u.login == inputLog && isPasswordCorrect);

                if (user != null)
                {
                    _cache.CacheUserId();
                    MessageBox.Show($"Успішний вхід!");

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    Close();
                }
                else
                {
                    MessageBox.Show("Невірний логін або пароль.");
                }
            }
            else
            {
                MessageBox.Show("Рядок порожній або містить лише пробіли.", "Помилка");
            }
        }
    }

    private void RegIn_Click(object sender, RoutedEventArgs e)
    {
        using (var context = new Context())
        {
            var dateNow = DateTime.UtcNow;
            string inputLog = txtLogin.Text;
            string inputPass = txtPassword.Text;
            string inputPassCopy = txtPassword_Copy.Text;
            string inputMail = txtMail.Text;

            bool check = IsValidEmail(inputMail);

            if (!(string.IsNullOrEmpty(inputLog) && string.IsNullOrEmpty(inputPass) && string.IsNullOrEmpty(inputPassCopy) && string.IsNullOrEmpty(inputMail)))
            {
                if (check)
                {
                    if (inputPass == inputPassCopy)
                    {
                        string salt = Protector.UseGenerateSalt();
                        string protectPassword = Protector.UseHashPassword(inputPass, salt);

                        var user = context.Users
                            .FirstOrDefault(u => u.login != inputLog);
                        if (user != null)
                        {
                            MessageBox.Show("Такий користувач вже зареєстрований");
                        }
                        else
                        {
                            var newUser = new User
                            {
                                login = inputLog,
                                email = inputMail,
                                password = protectPassword,
                                joing_date = dateNow,
                                salt = salt
                            };

                            context.Users.Add(newUser);
                            context.SaveChanges();

                            _cache.CacheUserId();

                            MessageBox.Show("Реєстрація пройшла успішно!");

                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();

                            Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Паролі не співпадають!", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Не правильно введена пошта!", "Error");
                }
            }
            else
            {
                MessageBox.Show("Заповнені не всі дані!", "Error");
            }
        }
    }

    private void GoToReg_Click(object sender, RoutedEventArgs e)
    {
        TabItem tab1 = LogAndReg.Items[0] as TabItem;
        TabItem tab2 = LogAndReg.Items[1] as TabItem;

        tab1.Visibility = Visibility.Collapsed;
        tab2.Visibility = Visibility.Visible;

        LogAndReg.SelectedItem = tab2;
    }

    private void BackToLog_Click(object sender, RoutedEventArgs e)
    {
        TabItem tab1 = LogAndReg.Items[0] as TabItem;
        TabItem tab2 = LogAndReg.Items[1] as TabItem;

        tab1.Visibility = Visibility.Visible;
        tab2.Visibility = Visibility.Collapsed;

        LogAndReg.SelectedItem = tab1;
    }
}