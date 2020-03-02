using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

using System.Diagnostics;

namespace ProcessNote
{

    public partial class MainWindow : Window
    {
        public List<DataGathering.ProcessItem> processAttributes = new List<DataGathering.ProcessItem>();
        DataGathering.ProcessData processData = new DataGathering.ProcessData();
        List<string> Comments = new List<string>();
        public List<Button> saveButtons = new List<Button>();
        public List<Button> cancelButtons = new List<Button>();
        public int newCommentsCount = 0;
        public bool popUpIsOpen = false;
        public string searchPage = "http://www.google.com";
  //      ThreadsPopUp threadsPopUp = new ThreadsPopUp();

        public MainWindow()
        {
            InitializeComponent();
            GetProcesses();            
        }

        public void GetProcesses()
        {
            foreach (Process process in processData.processes)
            {
                DataGathering.ProcessItem currentProcess = new DataGathering.ProcessItem
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    Process = process
                };
                this.ProcessList.Items.Add(currentProcess);
                currentProcess.SetRunningtTime();
                currentProcess.SetMemory();
                currentProcess.SetCPUUsage();
                currentProcess.SetThreads();
                processAttributes.Add(currentProcess);
            }
        }
        

        private void ListViewItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            if(newCommentsCount != 0)
            {
                CommentPopUp commentPopUp = new CommentPopUp();
                commentPopUp.Owner = this;
                commentPopUp.ShowDialog();
                popUpIsOpen = true;
            }
            

            
            var item = sender as ListViewItem;
            var dataContext = (DataGathering.ProcessItem)item.DataContext;


           if (item != null && item.IsSelected)
           {                
                this.ProcessDetails.Items.Clear();

                foreach (DataGathering.ProcessItem process in processAttributes)
                {
                    if (process.Id == dataContext.Id)
                    {
                        searchPage = $"https://www.google.com/search?q={process.Name}";
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("CPU Usage", process.CPUUsage));
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("Memory Usage", process.ConvertBytesToMB() ));
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("Running Time", process.GetRunningTime() ));
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("Start Time", process.GetStartTime() ));
                        CreateButton(process.GetThreads());
   //                     PopulateThreadListView(process.GetThreads(), threadsPopUp);
                    }
                }
                Comments = dataContext.Comments;
            }
            CommentGrid.Children.Clear();
            AddCommentButton(Comments);
            PopulateCommentGrid();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            var dataContext = (DataGathering.ProcessItem)item.DataContext;
            if (item != null && item.IsSelected)
            {
                this.ProcessDetails.Items.Clear();

                foreach (DataGathering.ProcessItem process in processAttributes)
                {
                    if (process.Id == dataContext.Id)
                    {
                        process.SetCPUUsage();
                        process.SetThreads();
                        process.SetMemory();
                        process.SetRunningtTime();
                        Thread.Sleep(2000);
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("CPU Usage", process.CPUUsage));
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("Memory Usage", process.ConvertBytesToMB()));
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("Running Time", process.GetRunningTime()));
                        this.ProcessDetails.Items.Add(new ProcessDetailRow("Start Time", process.GetStartTime()));
                    }
                }

            }
        }

        public void PopulateCommentGrid()
        {
            if (Comments != null)
            {
                for (int i = 0; i <= Comments.Count() - 1; i++)
                {
                    RowDefinition newRow = new RowDefinition();
                    newRow.Height = new GridLength(0, GridUnitType.Auto);
                    CommentGrid.RowDefinitions.Add(newRow);
                    TextBlock commentTextBlock = new TextBlock();
                    commentTextBlock.Text = Comments[i].ToString();
                    Grid.SetRow(commentTextBlock, i + 1);
                    Grid.SetColumn(commentTextBlock, 1);
                    CommentGrid.Children.Add(commentTextBlock);
                }
            }

        }

        public void AddComment(List<string> ProcessComments)
        {
            List<UIElement> elementsToRemove = new List<UIElement>();
            newCommentsCount++;
            int newRowIndex = CommentGrid.RowDefinitions.Count();
            RowDefinition newCommentRow = new RowDefinition();
            newCommentRow.Height = new GridLength(0, GridUnitType.Auto);
            CommentGrid.RowDefinitions.Add(newCommentRow);
            TextBox newComment = AddCommentTextBox(newRowIndex);
            elementsToRemove.Add(newComment);
            Button save = AddSaveButton(newRowIndex);
            elementsToRemove.Add(save);
            Button cancel = AddCancelButton(newRowIndex);
            elementsToRemove.Add(cancel);
            save.Click += delegate
            {
                saveButtonClick(newComment, newRowIndex);
                DeleteGridChildren(CommentGrid, elementsToRemove);
                ProcessComments.Add(newComment.Text);
                saveButtons.Remove(save);
                cancelButtons.Remove(cancel);
                newCommentsCount--;
                if(saveButtons == null)
                {
                    CommentGrid.Children.Clear();
                    PopulateCommentGrid();
                }
            };
            cancel.Click += delegate
            {
                DeleteGridChildren(CommentGrid, elementsToRemove);
                saveButtons.Remove(save);
                cancelButtons.Remove(cancel);
                newCommentsCount--;
            };
            

        }

        public void saveButtonClick(TextBox comment, int rowIndex)
        {
            string commentToAdd = comment.Text;
            TextBlock savedComment = new TextBlock();
            savedComment.Text = commentToAdd;
            Grid.SetRow(savedComment, rowIndex);
            Grid.SetColumn(savedComment, 1);
            if (!popUpIsOpen)
            {
                CommentGrid.Children.Add(savedComment);
            }
            
        }

        private void AddCommentButton(List<string> comments)
        {
            Button commentButton = new Button();
            commentButton.Content = "Add Comment";
            Grid.SetColumn(commentButton, 4);
            Grid.SetRow(commentButton, 0);
            CommentGrid.Children.Add(commentButton);
            commentButton.Click += delegate
            {
                AddComment(comments);
            };
        }

        private Button AddSaveButton(int rowIndex)
        {
            Button save = new Button();
            save.Name = "save" + newCommentsCount.ToString();
            save.Content = "Save";
            saveButtons.Add(save);
            Grid.SetRow(save, rowIndex);
            Grid.SetColumn(save, 2);
            CommentGrid.Children.Add(save);
            
            return save;
        }

        private Button AddCancelButton(int rowIndex)
        {
            Button cancel = new Button();
            cancel.Name = "cancel" + newCommentsCount.ToString();
            cancel.Content = "Cancel";
            Grid.SetRow(cancel, rowIndex);
            Grid.SetColumn(cancel, 3);
            CommentGrid.Children.Add(cancel);
            cancelButtons.Add(cancel);
            return cancel;
        }

        private TextBox AddCommentTextBox(int rowIndex)
        {
            TextBox newComment = new TextBox();
            newComment.Text = "New comment here";
            newComment.Name = "newComment" + newCommentsCount.ToString();
            Grid.SetRow(newComment, rowIndex);
            Grid.SetColumn(newComment, 1);
            CommentGrid.Children.Add(newComment);
            return newComment;
        }

        private void DeleteGridChildren(Grid grid, List<UIElement> toRemove)
        {
            foreach(UIElement element in toRemove)
            {
                grid.Children.Remove(element);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {            
            this.Topmost = true;
        }

        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }


        /*        private void ProcessList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            processAttributes.Clear();

            foreach (Process process in processData.processes)
            {
                DataGathering.ProcessItem currentProcess = new DataGathering.ProcessItem
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    Process = process
                };
                this.ProcessList.Items.Add(currentProcess);
                currentProcess.SetCPUUsage();
                currentProcess.SetRunningtTime();
                currentProcess.SetMemory();
                processAttributes.Add(currentProcess);
            }
        }*/


        public void CreateButton(ProcessThreadCollection threads)
        {
            Button button = new Button();
            button.Content = "Show threads";
            button.Name = "ThreadsButton";
            Grid.SetRow(button, 2);
            Grid.SetColumn(button, 2);
            button.Width = 100;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            MainGrid.Children.Add(button);
            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(button_Click));
            button.Click += delegate
            {
                ThreadsPopUp threadsPopUp = new ThreadsPopUp();
                threadsPopUp.Owner = this;
                PopulateThreadListView(threads, threadsPopUp);
                threadsPopUp.ShowDialog();
                
            };
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {

        }

        public void PopulateThreadListView(ProcessThreadCollection threads, ThreadsPopUp threadsPopUp)
        {
            
            if (threads != null)
            {
                foreach (ProcessThread thread in threads)
                {
                    threadsPopUp.ThreadList.Items.Add(thread);
                    
                }
            }
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(searchPage);
        }
    }
}
