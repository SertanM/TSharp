﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace TSharp
{
    // I will delete that in unity version :D
    internal abstract class Repl
    {
        private List<string> _submissionHistory = new List<string>();
        private int _submissionHistoryIndex;
        private bool _done;

        public void Run()
        {
            Console.ResetColor();

            while (true)
            {
                var text = EditSubmission();

                if (string.IsNullOrEmpty(text))
                    return;

                if (!text.Contains(Environment.NewLine) && text.StartsWith("#"))
                    EvaluateMetaCommand(text);
                else
                    EvaluateSubmission(text);

                _submissionHistory.Add(text);
                _submissionHistoryIndex = 0;
            }
        }


        private sealed class SubmissionView
        {
            private readonly Action<string> _lineRendered;
            private readonly ObservableCollection<string> _submissionDocument;
            private readonly int _cursorTop;
            private int _renderedLineCount;

            public SubmissionView(Action<string> lineRendered, ObservableCollection<string> submissionDocument)
            {
                _lineRendered = lineRendered;
                _submissionDocument = submissionDocument;
                _submissionDocument.CollectionChanged += SubmissionDocumentChanged;
                _cursorTop = Console.CursorTop;
                Render();
            }

            private void SubmissionDocumentChanged(object sender, NotifyCollectionChangedEventArgs e) 
                => Render();
            

            private void Render()
            {
                Console.CursorVisible = false;

                var lineCount = 0;

                foreach (var line in _submissionDocument)
                {
                    int cursorPos = _cursorTop + lineCount;
                    if (cursorPos >= 0 && cursorPos < Console.BufferHeight)
                    {
                        Console.SetCursorPosition(0, cursorPos);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;

                    if (lineCount == 0)
                        Console.Write("> ");
                    else
                        Console.Write(": ");

                    Console.ResetColor();

                    _lineRendered(line);

                    int spaceCount = Math.Max(0, Console.WindowWidth - line.Length);
                    Console.WriteLine(new string(' ', spaceCount));

                    lineCount++;
                }

                var numberOfBlankLines = _renderedLineCount - lineCount;
                if (numberOfBlankLines > 0)
                {
                    var blankLine = new string(' ', Console.WindowWidth);

                    for (var i = 0; i < numberOfBlankLines; i++)
                    {
                        int cursorPos = _cursorTop + lineCount + i;
                        if (cursorPos >= 0 && cursorPos < Console.BufferHeight)
                        {
                            Console.SetCursorPosition(0, cursorPos);
                            Console.WriteLine(blankLine);
                        }
                    }
                }

                _renderedLineCount = lineCount;

                Console.CursorVisible = true;
                UpdateCursorPosition();
            }

            private void UpdateCursorPosition()
            {
                int newTop = _cursorTop + CurrentLine;
                int newLeft = 2 + _currentCharacter;

                newTop = Math.Max(0, Math.Min(newTop, Console.BufferHeight - 1));
                newLeft = Math.Max(0, Math.Min(newLeft, Console.BufferWidth - 1));

                Console.CursorTop = newTop;
                Console.CursorLeft = newLeft;
            }


            private int _currentCharacter;
            private int _currentLine;

            public int CurrentLine 
            { 
                get => _currentLine;

                set 
                {
                    if (_currentLine == value)
                        return;

                    if (value < 0 || value >= _submissionDocument.Count)
                        return;

                    _currentLine = value;

                    _currentCharacter = Math.Min(
                        _submissionDocument[_currentLine].Length,
                        Math.Max(0, _currentCharacter)
                    );

                    UpdateCursorPosition();

                }
            }

            public int CurrentCharacter 
            {
                get => _currentCharacter;
                
                set 
                {
                    if (_currentCharacter == value) 
                        return;

                    _currentCharacter = value; 
                    UpdateCursorPosition();
                }
            }
        }


        private string EditSubmission()
        {
            _done = false;

            var document = new ObservableCollection<string>() { "" };
            var view = new SubmissionView(RenderLine, document);


            while (!_done) 
            {
                var key = Console.ReadKey(true);
                HandleKey(key, document, view);
            }

            view.CurrentLine = document.Count - 1;
            view.CurrentCharacter = document[view.CurrentLine].Length;

            Console.WriteLine();

            if (document.Count == 1 && document[0].Length == 0)
                return null;

            return string.Join(Environment.NewLine, document);
        }

        private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, SubmissionView view)
        {
            if (key.Modifiers == ConsoleModifiers.None)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        HandleEscape(document, view); 
                        break;
                    case ConsoleKey.Enter:
                        HandleEnter(document, view);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackSpace(document, view);
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete(document, view);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document, view);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document, view);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(document, view);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document, view);
                        break;
                    case ConsoleKey.Home:
                        HandleHome(document, view);
                        break;
                    case ConsoleKey.End:
                        HandleEnd(document, view);
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(document, view);
                        break;
                    case ConsoleKey.PageUp:
                        HandlePageUp(document, view);
                        break;
                    case ConsoleKey.PageDown:
                        HandlePageDown(document, view);
                        break;
                }
            }
            else if(key.Modifiers == ConsoleModifiers.Control)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleControlEnter(document, view);
                        break;
                }
            }

            if (key.KeyChar >= ' ')
                HandleTyping(document, view, key.KeyChar.ToString());
        }
        
        private void HandleEscape(ObservableCollection<string> document, SubmissionView view)
        {
            document.Clear();
            document.Add(string.Empty);
            view.CurrentLine = 0;
        }

        private void HandleEnter(ObservableCollection<string> document, SubmissionView view)
        {
            var submissionText = string.Join(Environment.NewLine, document);
            if (submissionText.StartsWith("#") || IsCompleteSubmission(submissionText))
            {
                _done = true;
                return;
            }

            InsertLine(document, view);
        }

        private void HandleControlEnter(ObservableCollection<string> document, SubmissionView view)
        {
            InsertLine(document, view);
        }

        private void InsertLine(ObservableCollection<string> document, SubmissionView view)
        {
            var remainder = document[view.CurrentLine].Substring(view.CurrentCharacter);
            document[view.CurrentLine] = document[view.CurrentLine].Substring(0, view.CurrentCharacter);

            var lineIndex = view.CurrentLine + 1;
            document.Insert(lineIndex, remainder);
            view.CurrentCharacter = 0;
            view.CurrentLine = lineIndex;
        }

        private void HandleBackSpace(ObservableCollection<string> document, SubmissionView view)
        {
            var start = view.CurrentCharacter;
            if (start == 0)
            {
                if(view.CurrentLine == 0)
                    return;

                var currentLine = document[view.CurrentLine];
                var previousLine = document[view.CurrentLine - 1];
                document.RemoveAt(view.CurrentLine);
                view.CurrentLine--;
                document[view.CurrentLine] = previousLine + currentLine;
                view.CurrentCharacter = previousLine.Length;
                return;
            }
            var lineIndex = view.CurrentLine;
            var line = document[lineIndex];
            

            var before = line.Substring(0, start - 1);
            var after = line.Substring(start);
            document[lineIndex] = before + after;
            view.CurrentCharacter--;
        }

        private void HandleDelete(ObservableCollection<string> document, SubmissionView view)
        {
            var lineIndex = view.CurrentLine;
            var line = document[lineIndex];
            var start = view.CurrentCharacter;

            if (start > line.Length - 1)
                return;

            var before = line.Substring(0, start);
            var after = line.Substring(start + 1);
            document[lineIndex] = before + after;
        }

        private void HandleLeftArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentCharacter > 0)
                view.CurrentCharacter--;
        }

        private void HandleRightArrow(ObservableCollection<string> document, SubmissionView view)
        {
            var line = document[view.CurrentLine];
            if (view.CurrentCharacter <= line.Length - 1)
                view.CurrentCharacter++;
        }

        private void HandleUpArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLine > 0)
                view.CurrentLine--;
        }

        private void HandleDownArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLine < document.Count - 1)
                view.CurrentLine++;
        }

        private void HandleEnd(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentCharacter = document[view.CurrentLine].Length;
        }

        private void HandleHome(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentCharacter = 0;
        }

        private void HandleTab(ObservableCollection<string> document, SubmissionView view)
        {
            const int TabWidth = 4;
            var start = view.CurrentCharacter;
            var remainingSpaces = TabWidth - start % TabWidth;

            var line = document[view.CurrentLine];
            document[view.CurrentLine] = line.Insert(start, new string(' ', remainingSpaces));
            view.CurrentCharacter += remainingSpaces;
        }

        private void HandlePageUp(ObservableCollection<string> document, SubmissionView view)
        {
            _submissionHistoryIndex--;
            if(_submissionHistoryIndex < 0)
                _submissionHistoryIndex = _submissionHistory.Count - 1;
            UpdateDocumentFromHistory(document, view);
        }

        private void HandlePageDown(ObservableCollection<string> document, SubmissionView view)
        {
            _submissionHistoryIndex++;
            if (_submissionHistoryIndex > _submissionHistory.Count - 1)
                _submissionHistoryIndex = 0;
            UpdateDocumentFromHistory(document, view);
        }

        private void UpdateDocumentFromHistory(ObservableCollection<string> document, SubmissionView view)
        {
            if (_submissionHistory.Count == 0)
                return;

            document.Clear();
            // I think that have some errors but I am an unity user not an code editor user LoL
            var historyItem = _submissionHistory[_submissionHistoryIndex];
            var lines = historyItem.Split(Environment.NewLine);
            
            foreach ( var line in lines )
                document.Add(line);

            view.CurrentLine = document.Count - 1;
            view.CurrentCharacter = document[view.CurrentLine].Length;
        }

        private void HandleTyping(ObservableCollection<string> document, SubmissionView view, string keyChar)
        {
            var lineIndex = view.CurrentLine;
            var start = view.CurrentCharacter;
            document[lineIndex] = document[lineIndex].Insert(start, keyChar);
            view.CurrentCharacter += keyChar.Length;
        }

        private string EditSubmissionOld()
        {
            StringBuilder _textBuilder = new StringBuilder();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                if (_textBuilder.Length == 0)
                    Console.Write("> ");
                else
                    Console.Write(": ");

                Console.ResetColor();

                var input = Console.ReadLine();
                bool isBlank = string.IsNullOrEmpty(input);


                if (_textBuilder.Length == 0)
                {
                    if (isBlank)
                        return null;


                    if (input.StartsWith("#"))
                    {
                        EvaluateMetaCommand(input);
                        continue;
                    }
                }

                _textBuilder.Append(input);
                var text = _textBuilder.ToString();

                if (!IsCompleteSubmission(text))
                    continue;

                return text;
            }
        }


        protected void ClearHistory()
        {
            _submissionHistory.Clear();
        }

        protected virtual void RenderLine(string line)
        {
            Console.Write(line);
        }

        protected virtual void EvaluateMetaCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Invalid command {input}");
        }

        protected abstract bool IsCompleteSubmission(string text);

        protected abstract void EvaluateSubmission(string text);

    }
}