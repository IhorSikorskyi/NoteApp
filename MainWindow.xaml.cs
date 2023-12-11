using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Ecng.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NoteApp;

namespace NoteAppWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Cache _cache;

    public MainWindow(Cache cache)
    {
        _cache = cache;
    }

    public MainWindow()
    {
        InitializeComponent();

        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
        timer.Start();

        UpdateClock();

        _cache = new Cache(new Context(), new MemoryCache(new MemoryCacheOptions()));
        List<Category> data = _cache.GetDataFromDatabase();

        dataGrid.ItemsSource = data;
    }

    private void save_Click(object sender, RoutedEventArgs e)
    {

    }

    private void read_Click(object sender, RoutedEventArgs e)
    {

    }

    private void update_Click(object sender, RoutedEventArgs e)
    {

    }

    private void delete_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        txtClock.Text = DateTime.Now.ToString("HH:mm:ss");
    }

    private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void clear_Click_1(object sender, RoutedEventArgs e)
    {
        txtTitle.Clear();
        txtMessage.Clear();
    }

    private void LogOut_Click(object sender, RoutedEventArgs e)
    {
        _cache.DeleteCachedUserId();
        Close();
    }

    private void CatCreate_Click(object sender, RoutedEventArgs e)
    {
        using (var context = new Context())
        {
            string categoryName = txtCategory1.Text;
            int currentUserId = _cache.GetCachedUserId();

            var user = context.Users.FirstOrDefault(u => u.user_id == currentUserId);

            if (user != null)
            {
                var newCategory = new Category
                {
                    name = categoryName,
                    userid = user.user_id
                };

                context.Categories.Add(newCategory);
                context.SaveChanges();

                MessageBox.Show("Категорія додана успішно!");
            }
            
        }
    }

    private void catUpdate_Click(object sender, RoutedEventArgs e)
    {
        using (var context = new Context())
        {
            string categoryName = txtCategory1.Text;
            if (categoryName != string.Empty)
            {
                var selectedCategory = context.Categories
                    .FirstOrDefault(c => c.name != categoryName);

                selectedCategory.name = categoryName;

                context.Categories.Update(selectedCategory);
                context.SaveChanges();

            }
        }
    }

    private void catRead_Click(object sender, RoutedEventArgs e)
    {
        using (var context = new Context())
        {
            var selectedCategory = dataGrid.SelectedItems as Category;

            if (selectedCategory != null)
            {
                txtCategory1.Text = selectedCategory.name;
            }
        }
    }

    private void catDelete_Click(object sender, RoutedEventArgs e)
    {
        using (var context = new Context())
        {
            string categoryName = txtCategory1.Text;
            var selectedCategory = dataGrid.SelectedItems as Category;
            var categoryToDelete = context.Categories
                .FirstOrDefault(c => c.categoryid == selectedCategory.categoryid);
            if (categoryToDelete != null && categoryName != null)
            {
                context.Categories.Remove(categoryToDelete);
                context.SaveChanges();
            }
            else
            {
                MessageBox.Show("Такої категорії не існує!");
            }
        }
    }
}