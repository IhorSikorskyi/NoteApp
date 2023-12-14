using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using NoteApp;
using StockSharp.Messages;

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
    public ObservableCollection<Note> NotesForView { get; set; }

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
        NotesForView = new ObservableCollection<Note>(_cache.GetDataFromNotes());
        comboCategory.ItemsSource = new ObservableCollection<Category>(_cache.GetDataFromDatabase());
        comboCategory.DisplayMemberPath = "name";
    }

    private void create_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
        var userId = _cache.GetCachedUserId();

        string inputTitle = txtTitle.Text;
        string inputMessage = txtMessage.Text;

        int selectedCategoryId = ((Category)comboCategory.SelectedItem).categoryid;

        var newNote = new Note()
        {
            title = inputTitle,
            message = inputMessage,
            creationdate = DateTime.UtcNow,
            category_id = selectedCategoryId, //заглушка
            userid = userId
        };

        context.Notes.Add(newNote);
        context.SaveChanges();

        NotesForView.Clear();
        foreach (var note in context.Notes.ToList())
        {
            NotesForView.Add(note);
        }

        txtTitle.Clear();
        txtMessage.Clear();
    }

    private void update_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
        Note selectedNote = (Note)notesGrid.SelectedItem;
        var userId = _cache.GetCachedUserId();

        int selectedCategoryId = ((Category)comboCategory.SelectedItem).categoryid;

        if (selectedNote != null)
        {
            string inputTitle = txtTitle.Text;
            string inputMessage = txtMessage.Text;

            selectedNote.title = inputTitle;
            selectedNote.message = inputMessage;
            selectedNote.creationdate = DateTime.UtcNow;
            selectedNote.category_id = selectedCategoryId;
            selectedNote.userid = userId;

            context.Notes.Update(selectedNote);
            context.SaveChanges();

            NotesForView.Clear();
            foreach (var note in context.Notes.ToList())
            {
                NotesForView.Add(note);
            }

            txtTitle.Clear();
            txtMessage.Clear();

        }
    }

    private void read_Click(object sender, RoutedEventArgs e)
    {
        Note selectedNote = (Note)notesGrid.SelectedItem;

        if (selectedNote != null)
        {
            txtTitle.Text = selectedNote.title;
            txtMessage.Text = selectedNote.message;
        }
    }

    private void delete_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
        Note selectedNote = (Note)notesGrid.SelectedItem;

        if (selectedNote != null)
        {
            context.Notes.Remove(selectedNote);
            context.SaveChanges();

            NotesForView.Clear();
            foreach (var note in context.Notes.ToList())
            {
                NotesForView.Add(note);
            }

            txtTitle.Clear();
            txtMessage.Clear();
        }
    }

    private void clear_Click_1(object sender, RoutedEventArgs e)
    {
        txtTitle.Clear();
        txtMessage.Clear();
    }

    private void toFind_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();

        if (int.TryParse(txtSearch.Text, out int searchId))
        {
            var foundNote = context.Notes.FirstOrDefault(n => n.noteid == searchId);

            if (foundNote != null)
            {
                txtTitle.Text = foundNote.title;
                txtMessage.Text = foundNote.message;

                var categoriesList = new List<Category>(CategoriesForView);


                int categoryIndex = categoriesList.FindIndex(c => c.categoryid == foundNote.category_id);

                comboCategory.SelectedIndex = categoryIndex;
            }

        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        txtClock.Text = DateTime.Now.ToString("HH:mm:ss");
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

        var newCategory = new Category
        {
            name = categoryName,
            userid = currentUserId
        };

        context.Categories.Add(newCategory);
        context.SaveChanges();

        CategoriesForView.Clear();
        foreach (var cat in context.Categories.ToList())
        {
            CategoriesForView.Add(cat);
        }

        RefreshComboBox();
        txtCategory1.Clear();
    }

    private void catUpdate_Click(object sender, RoutedEventArgs e)
    {

        using var context = new Context();

        Category selectedCategory = (Category)dataGrid.SelectedItem;

        if (selectedCategory != null)
        {
            string categoryName = txtCategory1.Text;

            selectedCategory.name = categoryName;
            selectedCategory.userid = _cache.GetCachedUserId();
            
            context.Categories.Update(selectedCategory);
            context.SaveChanges();

            CategoriesForView.Clear();
            foreach (var cat in context.Categories.ToList())
            {
                CategoriesForView.Add(cat);
            }

            RefreshComboBox();
            txtCategory1.Clear();
        }
        else
        {
            MessageBox.Show("Будь ласка, оберіть категорію з датагріда.");
        }

    }

    private void catRead_Click(object sender, RoutedEventArgs e)
    {
        Category selectedCategory = (Category)dataGrid.SelectedItem;

        if (selectedCategory != null)
        {
            txtCategory1.Text = selectedCategory.name;
        }
    }

    private void catDelete_Click(object sender, RoutedEventArgs e)
    {
        using var context = new Context();
        Category selectedCategory = (Category)dataGrid.SelectedItem;

        if (selectedCategory != null)
        {
            context.Categories.Remove(selectedCategory);
            context.SaveChanges();

            CategoriesForView.Clear();
            foreach (var cat in context.Categories.ToList())
            {
                CategoriesForView.Add(cat);
            }

            txtCategory1.Clear();
        }
    }

    private void RefreshComboBox()
    {
        NotesForView = new ObservableCollection<Note>(_cache.GetDataFromNotes());
        comboCategory.ItemsSource = new ObservableCollection<Category>(_cache.GetDataFromDatabase());
        comboCategory.DisplayMemberPath = "name";
    }
}