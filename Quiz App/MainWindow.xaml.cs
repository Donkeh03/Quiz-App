using Quiz_App;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace Quiz_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    

    public class Question
    {
        public string? QuestionText { get; set; } // The text of the question, like "What is 3 x 3?"
        public string? CorrectAnswerIndex { get; set; } // The "index" (number) that is the correct anwser, between 1 and 4
        public string? Answers { get; set; } // A formatted string that represnts the possible anwsers, its a formatted string as it easier to write than an array, might be more memory efficent.
    }
    //The bulk of the logic for the system:
    //each "question" is shown question object, a group of questions is stored in the "Quiz" class

    //The quiz class, stores the name of the quiz and the questions in a single object
    public class Quiz
    {
        public List<Question>? QuizQuestions { get; set; } //list of the questions
        public string? QuizName { get; set; } // the name of the quiz
    }

    public class User
    {
        public string? name { get; set; }
        public int? score { get; set; }
    }

    //global access to the quiz data
    public partial class App : Application
    {
        public List<Quiz>? GlobalQuizzes { get; set; }

        public List<string>? GlobalLeaderPaths { get; set; }
    }

    public partial class MainWindow : Window
    {

        public static string QuizDir = @"..\..\JSONquiz\"; // the dir the program looks in to get the json files. update as needed where-ever the json file are.
        public static string LeaderDir = @"..\..\LeaderJson\"; // the dir the program looks in to get the json files. update as needed where-ever the json file are.

        //once Init is done, page 1 (main menu) is loaded. start of page functionaility
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.NavigationService.Navigate(new Page1());
        }

        private List<Quiz> LoadQuizes() // loads the JSON files and returns to "Quizzes" variable in the load function
        {
            List<string> files = ScanDirectoryForJsonFiles(QuizDir); // the actual scanning part, list of file name to open below

            List<Quiz> quizzes = new List<Quiz>(); //the quiz list, local to the scope of this function, returned at the end

            foreach (string file in files) // looping through the list of files that were scanned earlier
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file); // constructs the file path to the file
                string fileText = File.ReadAllText(filePath); //reads everything in said file

                Question[] questions = JsonSerializer.Deserialize<Question[]>(fileText); // in-line list comprehesion of the JSON file, as question objects, put into a list

                Quiz quiz = new Quiz //new quiz made
                {
                    QuizQuestions = questions.ToList(), //the list of questions for the quiz
                    QuizName = System.IO.Path.GetFileNameWithoutExtension(file) //the name of the file (auto removes the .JSON ending)
                };

                quizzes.Add(quiz); // adding quiz to the list of quizzes
            }

            return quizzes; // returns the data to the load function
        }

        private List<string> ScanDirectoryForJsonFiles(string directoryPath) //Scans the dir for JSON files, returns the file names as a list of strings
        {
            List<string> jsonFiles = new List<string>(); // init the string list

            try
            {
                // Get all files with the .json extension in the specified directory
                string[] files = Directory.GetFiles(directoryPath, "*.json");

                // Add the file paths to the list
                jsonFiles.AddRange(files);
            }
            catch (DirectoryNotFoundException) // not found exception
            {
                MessageBox.Show("Directory Not found", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error); // used to display errors. ::msg to user, title of box, option, img
            }
            catch (Exception ex) // scan error
            {

                MessageBox.Show($"Error While Scanning {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error); // used to display errors. ::msg to user, title of box, option, img
            }

            return jsonFiles; // return all files found to the LoadQuizs function
        }

        public static List<string> ScanQuizDir()
        {
            List<string> QuizFileNames = new List<string>();

            foreach (string path in Directory.GetFiles(QuizDir))
            {
                string[] parts = path.Split(@"\");
                string FileName = parts[(parts.Length - 1)];

                QuizFileNames.Add(FileName);
            }
            return QuizFileNames;
        }

        public static List<string> LeadDirScan(List<string> Files)
        {
            List<string> LeaderFilePaths = new List<string>();

            foreach (string file in Files)
            {
                
                
                string FilePath = $"{LeaderDir}Leader{file}";
                bool result = File.Exists(FilePath);

               
                if (result == false)
                {
                    using (var stream = File.Create(FilePath)) { }
                    string jsonContent = "[]";
                    File.WriteAllText(FilePath, jsonContent);
                }

                LeaderFilePaths.Add(FilePath);
            }

            return LeaderFilePaths;
        }

        private void Init(object sender, RoutedEventArgs e)
        {
            List<Quiz> Quizzes = LoadQuizes();
            ((App)Application.Current).GlobalQuizzes = Quizzes;

            List<string> QuizFileNames = ScanQuizDir();
            List<string> LeaderJSONs = LeadDirScan(QuizFileNames);
            ((App)Application.Current).GlobalLeaderPaths = LeaderJSONs;
        }
    }
}