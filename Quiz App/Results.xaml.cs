using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Shapes;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;

namespace Quiz_App
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Page
    {
        public object Data { get; set; } // very important, holds the quiz needed and the name of the quiz

        public Results(string Name, int score, int len)
        {
            InitializeComponent();
            Data = new { Name, score, len };
        }


        public static void AddToLeaders(string FileName, List<string> FilePaths, int PlrScore, string PlrName) //if the name parsed is found, update the score of that user object and return, if no name is found make a new obejct and return. keep save logic where it is, return json deserialized
        {
            List<User> UpdateLeader(string PlrName, int PlrScore, List<User> FileContent) // this is a small inner function that take the players name, their score and the content of a JSON file
            {
                foreach (User user in FileContent) // loop over the content of the file
                {
                    if (user.name == PlrName) //if the name parsed is found
                    {
                        if (PlrScore > user.score)
                        { // and the score they just got is higher than their current score
                            user.score = PlrScore; // update it
                            return FileContent; // return the file
                        }
                        else
                        {
                            return FileContent;
                        }
                    }
                }
                //if the player isnt found make a new user obj with the parsed data, added to the file
                FileContent.Add(new User()
                {
                    name = PlrName,
                    score = PlrScore
                });

                return FileContent; // return file
            }

            List<User>? JSONcontent = default;
            string path = null;

            foreach (string file in FilePaths)
            {
                string[] parts = file.Split(@"\");
                string name = parts[(parts.Length - 1)];
                
                if (name == FileName)
                {
                    path = file;
                    string content = File.ReadAllText(file);
                    JSONcontent = JsonSerializer.Deserialize<List<User>>(content);
                    break;
                }
            }

            if (path == null)
            {
                MessageBox.Show($"Unable to find Leaderboard File!", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error); // used to display errors. ::msg to user, title of box, option, img
                return;
            }

            List<User> UpdatedContent = UpdateLeader(PlrName, PlrScore, JSONcontent);

            List<User> SortedContent = UpdatedContent.OrderByDescending(User => User.score).ToList();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            string serializedContent = JsonSerializer.Serialize(SortedContent, options);

            File.WriteAllText(path, serializedContent);
        }

        private void RetakeQuiz_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new QuizSelect());
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1());
        }

        private string ValidateUsernameInput(string username)
        {
            List<string> BadWords = new List<string> { 
            "Fuck",
            "Shit",
            "Penis",
            "Tit",
            };


            if (username.Length > 0 && username.Length <= 8)
            {
             foreach (string word in BadWords)
                {
                    string LowercaseName = username.ToLower();
                    if (LowercaseName.Contains(word.ToLower()))
                    {
                        return "";
                    }
                }
                return username;
            }
            return "";
        }

        private void LeaderAdd_Click(object sender, RoutedEventArgs e)
        {
            var data = (dynamic)Data; // gets internal data
            string FilePath = $"Leader{data.Name}.json";
            List<string>? FilePaths = ((App)Application.Current).GlobalLeaderPaths;
            int PlrScore = data.score;
            string PlrName = ValidateUsernameInput(PlayerUsernameTxtBox.Text);

            if (!string.IsNullOrEmpty(PlrName)) {
                AddToLeaders(FilePath, FilePaths, PlrScore, PlrName);
                this.NavigationService.Navigate(new QuizSelect());
            }
            else
            {
                MessageBox.Show("Bad Username", "Username Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void setGUI(object sender, RoutedEventArgs e)
        {
            var data = (dynamic)Data; // gets data
            string ScoreString = $"{data.score}/{data.len}";
            ScoreLbl.Content = $"Your Score : {ScoreString}";
        }

        private void OnKeyPress(object sender, RoutedEventArgs e)
        {
            var Btn = (Button)sender;

            if (Btn.Content != null)
            {
                if (PlayerUsernameTxtBox.Text.Length < 8)
                {
                    PlayerUsernameTxtBox.Text += Btn.Content;
                }
            }
        }


        private void PlayerUsernameTxtBox_TextFocused(object sender, RoutedEventArgs e)
        {
            if (PlayerUsernameTxtBox.Text == "Enter Your Name. . .")
            {
                PlayerUsernameTxtBox.Text = "";
            }
            
            KeyboardBackingBorder.Visibility = Visibility.Visible;
        }

        private void PlayerUsernameTxtBox_TextUnfocused(object sender, RoutedEventArgs e)
        {
            
            KeyboardBackingBorder.Visibility = Visibility.Hidden;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(PlayerUsernameTxtBox), null);
            Keyboard.ClearFocus();
            if (string.IsNullOrEmpty(PlayerUsernameTxtBox.Text))
            {
                PlayerUsernameTxtBox.Text = "Enter Your Name. . .";
            }
        }

        private void KeyboardKey_del_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerUsernameTxtBox.Text.Count() > 0)
            {
                string RemovedChar = PlayerUsernameTxtBox.Text.Remove(PlayerUsernameTxtBox.Text.Length - 1);
                PlayerUsernameTxtBox.Text = RemovedChar;
            }
        }

        private void KeyboardKey_Caps_Click(object sender, RoutedEventArgs e)
        {
            Regex LowerCaseCheckCondition = new Regex("^[a-z]*$");
            Regex UpperCaseCheckCondition = new Regex("^[A-Z]*$");

            void ChangeCaseOfChildElements(DockPanel obj)
            {
                foreach (Border OuterKeyBorder in obj.Children)
                {
                    Button Key = (Button)OuterKeyBorder.Child;
                    
                    string? KeyText = Key.Content.ToString();
                    if (KeyText != null)
                    {
                        if (LowerCaseCheckCondition.IsMatch(KeyText))
                        {
                            Key.Content = KeyText.ToUpper();

                        }
                        else if (UpperCaseCheckCondition.IsMatch(KeyText))
                        {
                            Key.Content = KeyText.ToLower();
                        }
                    }
                }
            }

            ChangeCaseOfChildElements(TopRow);
            ChangeCaseOfChildElements(MidTopRow);
            ChangeCaseOfChildElements(MidBtmRow);
        }
    }
}
