using System.Collections.ObjectModel;
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

    public ObservableCollection<Category> CategoriesForView { get; set; }

    public MainWindow()
    {
        InitializeComponent();

        _cache = (Cache)App.ServiceProvider.GetService(typeof(Cache));

        DataContext = this;
        LoadData();

        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
        timer.Start();

        UpdateClock();

    }

    private void LoadData()
    {
        _cache.CacheUserId();
        CategoriesForView = new ObservableCollection<Category>(_cache.GetDataFromDatabase());
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

    private void clear_Click_1(object sender, RoutedEventArgs e)
    {
        txtTitle.Clear();
        txtMessage.Clear();
    }

    private void toFind_Click(object sender, RoutedEventArgs e)
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

    private void LogOut_Click(object sender, RoutedEventArgs e)
    {
        _cache.DeleteCachedUserId();
        Close();
    }

    private void CatCreate_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
        string categoryName = txtCategory1.Text;
        int currentUserId = _cache.GetCachedUserId();

        var category = context.Categories.FirstOrDefault(c => c.name != categoryName);

        if (category != null)
        {
            var newCategory = new Category
            {
                name = categoryName,
                userid = currentUserId
            };

            context.Categories.Add(newCategory);
            context.SaveChanges();

            txtCategory1.Clear();
        }
    }

    private void catUpdate_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
        string categoryName = txtCategory1.Text;
    }

    private void catRead_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
    }

    private void catDelete_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
        string categoryName = txtCategory1.Text;
        var categoryToDelete = context.Categories
            .FirstOrDefault(c => c.categoryid == 12);
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