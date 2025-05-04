using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ParallelAsyncExample
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client = new HttpClient { MaxResponseContentBufferSize = 1_000_000 };

        private readonly IEnumerable<string> _urlList = new string[]
        {
            "https://docs.microsoft.com",
            "https://docs.microsoft.com/azure",
            "https://docs.microsoft.com/powershell",
            "https://docs.microsoft.com/dotnet",
            "https://docs.microsoft.com/aspnet/core",
            "https://docs.microsoft.com/windows",
            "https://docs.microsoft.com/office",
            "https://docs.microsoft.com/enterprise-mobility-security",
            "https://docs.microsoft.com/visualstudio",
            "https://docs.microsoft.com/microsoft-365",
            "https://docs.microsoft.com/sql",
            "https://docs.microsoft.com/dynamics365",
            "https://docs.microsoft.com/surface",
            "https://docs.microsoft.com/xamarin",
            "https://docs.microsoft.com/azure/devops",
            "https://docs.microsoft.com/system-center",
            "https://docs.microsoft.com/graph",
            "https://docs.microsoft.com/education",
            "https://docs.microsoft.com/gaming"
        };

        /// <summary>        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Linq;
        using System.Net.Http;
        using System.Threading.Tasks;
        using System.Windows;
        using System.Windows.Threading;
        
        namespace ParallelAsyncExample
        {
            public partial class MainWindow : Window
            {
                // HttpClient instance used to fetch data from URLs.
                // The MaxResponseContentBufferSize limits the size of the response buffer.
                private readonly HttpClient _client = new HttpClient { MaxResponseContentBufferSize = 1_000_000 };
        
                // A predefined list of URLs to fetch data from.
                private readonly IEnumerable<string> _urlList = new string[]
                {
                    "https://docs.microsoft.com",
                    "https://docs.microsoft.com/azure",
                    "https://docs.microsoft.com/powershell",
                    "https://docs.microsoft.com/dotnet",
                    "https://docs.microsoft.com/aspnet/core",
                    "https://docs.microsoft.com/windows",
                    "https://docs.microsoft.com/office",
                    "https://docs.microsoft.com/enterprise-mobility-security",
                    "https://docs.microsoft.com/visualstudio",
                    "https://docs.microsoft.com/microsoft-365",
                    "https://docs.microsoft.com/sql",
                    "https://docs.microsoft.com/dynamics365",
                    "https://docs.microsoft.com/surface",
                    "https://docs.microsoft.com/xamarin",
                    "https://docs.microsoft.com/azure/devops",
                    "https://docs.microsoft.com/system-center",
                    "https://docs.microsoft.com/graph",
                    "https://docs.microsoft.com/education",
                    "https://docs.microsoft.com/gaming"
                };
        
                /// <summary>
                /// Handles the click event for the Start button.
                /// Disables the Start button, clears the results text box, 
                /// and initiates an asynchronous task to start summing page sizes.
                /// </summary>
                /// <param name="sender">The source of the event, typically the Start button.</param>
                /// <param name="e">The event data associated with the click event.</param>
                private void OnStartButtonClick(object sender, RoutedEventArgs e)
                {
                    // Disable the Start button to prevent multiple clicks during the operation.
                    _startButton.IsEnabled = false;
        
                    // Clear the results text box to prepare for new output.
                    _resultsTextBox.Clear();
        
                    // Start the asynchronous operation to sum page sizes on a background thread.
                    // This ensures the UI remains responsive during the operation.
                    Task.Run(() => StartSumPageSizesAsync());
                }
        
                /// <summary>
                /// Initiates the process of summing page sizes asynchronously.
                /// Updates the UI with the results and re-enables the Start button upon completion.
                /// </summary>
                private async Task StartSumPageSizesAsync()
                {
                    // Perform the asynchronous operation to sum page sizes.
                    await SumPageSizesAsync();
        
                    // Use the Dispatcher to update the UI safely from the background thread.
                    await Dispatcher.BeginInvoke(() =>
                    {
                        // Append a message indicating that control has returned to the event handler.
                        _resultsTextBox.Text += $"\nControl returned to {nameof(OnStartButtonClick)}.";
        
                        // Re-enable the Start button to allow the user to start another operation.
                        _startButton.IsEnabled = true;
                    });
                }
        
                /// <summary>
                /// Fetches the sizes of web pages in parallel and calculates the total size.
                /// Updates the UI with the total size and elapsed time.
                /// </summary>
                private async Task SumPageSizesAsync()
                {
                    // Start a stopwatch to measure the elapsed time.
                    var stopwatch = Stopwatch.StartNew();
        
                    // Create a query to process each URL asynchronously.
                    IEnumerable<Task<int>> downloadTasksQuery =
                        from url in _urlList
                        select ProcessUrlAsync(url, _client);
        
                    // Convert the query to an array of tasks.
                    Task<int>[] downloadTasks = downloadTasksQuery.ToArray();
        
                    // Wait for all tasks to complete and retrieve their results.
                    int[] lengths = await Task.WhenAll(downloadTasks);
                    int total = lengths.Sum();
        
                    // Update the UI with the total size and elapsed time.
                    await Dispatcher.BeginInvoke(() =>
                    {
                        stopwatch.Stop();
        
                        _resultsTextBox.Text += $"\nTotal bytes returned:  {total:#,#}";
                        _resultsTextBox.Text += $"\nElapsed time:          {stopwatch.Elapsed}\n";
                    });
                }
        
                /// <summary>
                /// Downloads the content of a URL and returns its size.
                /// Updates the UI with the size of the downloaded content.
                /// </summary>
                /// <param name="url">The URL to process.</param>
                /// <param name="client">The HttpClient instance used for the request.</param>
                /// <returns>The size of the downloaded content in bytes.</returns>
                private async Task<int> ProcessUrlAsync(string url, HttpClient client)
                {
                    // Download the content of the URL as a byte array.
                    byte[] byteArray = await client.GetByteArrayAsync(url);
        
                    // Display the results in the UI.
                    await DisplayResultsAsync(url, byteArray);
        
                    // Return the size of the downloaded content.
                    return byteArray.Length;
                }
        
                /// <summary>
                /// Updates the UI with the size of the content downloaded from a URL.
                /// </summary>
                /// <param name="url">The URL that was processed.</param>
                /// <param name="content">The downloaded content as a byte array.</param>
                /// <returns>A Task representing the asynchronous operation.</returns>
                private Task DisplayResultsAsync(string url, byte[] content) =>
                    Dispatcher.BeginInvoke(() =>
                        _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}\n")
                              .Task;
        
                /// <summary>
                /// Cleans up resources when the window is closed.
                /// Disposes of the HttpClient instance to release unmanaged resources.
                /// </summary>
                /// <param name="e">The event data associated with the window closing event.</param>
                protected override void OnClosed(EventArgs e) => _client.Dispose();
            }
        }        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Linq;
        using System.Net.Http;
        using System.Threading.Tasks;
        using System.Windows;
        using System.Windows.Threading;
        
        namespace ParallelAsyncExample
        {
            public partial class MainWindow : Window
            {
                // HttpClient instance used to fetch data from URLs.
                // The MaxResponseContentBufferSize limits the size of the response buffer.
                private readonly HttpClient _client = new HttpClient { MaxResponseContentBufferSize = 1_000_000 };
        
                // A predefined list of URLs to fetch data from.
                private readonly IEnumerable<string> _urlList = new string[]
                {
                    "https://docs.microsoft.com",
                    "https://docs.microsoft.com/azure",
                    "https://docs.microsoft.com/powershell",
                    "https://docs.microsoft.com/dotnet",
                    "https://docs.microsoft.com/aspnet/core",
                    "https://docs.microsoft.com/windows",
                    "https://docs.microsoft.com/office",
                    "https://docs.microsoft.com/enterprise-mobility-security",
                    "https://docs.microsoft.com/visualstudio",
                    "https://docs.microsoft.com/microsoft-365",
                    "https://docs.microsoft.com/sql",
                    "https://docs.microsoft.com/dynamics365",
                    "https://docs.microsoft.com/surface",
                    "https://docs.microsoft.com/xamarin",
                    "https://docs.microsoft.com/azure/devops",
                    "https://docs.microsoft.com/system-center",
                    "https://docs.microsoft.com/graph",
                    "https://docs.microsoft.com/education",
                    "https://docs.microsoft.com/gaming"
                };
        
                /// <summary>
                /// Handles the click event for the Start button.
                /// Disables the Start button, clears the results text box, 
                /// and initiates an asynchronous task to start summing page sizes.
                /// </summary>
                /// <param name="sender">The source of the event, typically the Start button.</param>
                /// <param name="e">The event data associated with the click event.</param>
                private void OnStartButtonClick(object sender, RoutedEventArgs e)
                {
                    // Disable the Start button to prevent multiple clicks during the operation.
                    _startButton.IsEnabled = false;
        
                    // Clear the results text box to prepare for new output.
                    _resultsTextBox.Clear();
        
                    // Start the asynchronous operation to sum page sizes on a background thread.
                    // This ensures the UI remains responsive during the operation.
                    Task.Run(() => StartSumPageSizesAsync());
                }
        
                /// <summary>
                /// Initiates the process of summing page sizes asynchronously.
                /// Updates the UI with the results and re-enables the Start button upon completion.
                /// </summary>
                private async Task StartSumPageSizesAsync()
                {
                    // Perform the asynchronous operation to sum page sizes.
                    await SumPageSizesAsync();
        
                    // Use the Dispatcher to update the UI safely from the background thread.
                    await Dispatcher.BeginInvoke(() =>
                    {
                        // Append a message indicating that control has returned to the event handler.
                        _resultsTextBox.Text += $"\nControl returned to {nameof(OnStartButtonClick)}.";
        
                        // Re-enable the Start button to allow the user to start another operation.
                        _startButton.IsEnabled = true;
                    });
                }
        
                /// <summary>
                /// Fetches the sizes of web pages in parallel and calculates the total size.
                /// Updates the UI with the total size and elapsed time.
                /// </summary>
                private async Task SumPageSizesAsync()
                {
                    // Start a stopwatch to measure the elapsed time.
                    var stopwatch = Stopwatch.StartNew();
        
                    // Create a query to process each URL asynchronously.
                    IEnumerable<Task<int>> downloadTasksQuery =
                        from url in _urlList
                        select ProcessUrlAsync(url, _client);
        
                    // Convert the query to an array of tasks.
                    Task<int>[] downloadTasks = downloadTasksQuery.ToArray();
        
                    // Wait for all tasks to complete and retrieve their results.
                    int[] lengths = await Task.WhenAll(downloadTasks);
                    int total = lengths.Sum();
        
                    // Update the UI with the total size and elapsed time.
                    await Dispatcher.BeginInvoke(() =>
                    {
                        stopwatch.Stop();
        
                        _resultsTextBox.Text += $"\nTotal bytes returned:  {total:#,#}";
                        _resultsTextBox.Text += $"\nElapsed time:          {stopwatch.Elapsed}\n";
                    });
                }
        
                /// <summary>
                /// Downloads the content of a URL and returns its size.
                /// Updates the UI with the size of the downloaded content.
                /// </summary>
                /// <param name="url">The URL to process.</param>
                /// <param name="client">The HttpClient instance used for the request.</param>
                /// <returns>The size of the downloaded content in bytes.</returns>
                private async Task<int> ProcessUrlAsync(string url, HttpClient client)
                {
                    // Download the content of the URL as a byte array.
                    byte[] byteArray = await client.GetByteArrayAsync(url);
        
                    // Display the results in the UI.
                    await DisplayResultsAsync(url, byteArray);
        
                    // Return the size of the downloaded content.
                    return byteArray.Length;
                }
        
                /// <summary>
                /// Updates the UI with the size of the content downloaded from a URL.
                /// </summary>
                /// <param name="url">The URL that was processed.</param>
                /// <param name="content">The downloaded content as a byte array.</param>
                /// <returns>A Task representing the asynchronous operation.</returns>
                private Task DisplayResultsAsync(string url, byte[] content) =>
                    Dispatcher.BeginInvoke(() =>
                        _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}\n")
                              .Task;
        
                /// <summary>
                /// Cleans up resources when the window is closed.
                /// Disposes of the HttpClient instance to release unmanaged resources.
                /// </summary>
                /// <param name="e">The event data associated with the window closing event.</param>
                protected override void OnClosed(EventArgs e) => _client.Dispose();
            }
        }
        /// Handles the click event for the Start button.
        /// Disables the Start button, clears the results text box, 
        /// and initiates an asynchronous task to start summing page sizes.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Start button.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            _startButton.IsEnabled = false;
            _resultsTextBox.Clear();

            Task.Run(() => StartSumPageSizesAsync());
        }

        private async Task StartSumPageSizesAsync()
        {
            await SumPageSizesAsync();
            await Dispatcher.BeginInvoke(() =>
            {
                _resultsTextBox.Text += $"\nControl returned to {nameof(OnStartButtonClick)}.";
                _startButton.IsEnabled = true;
            });
        }

        private async Task SumPageSizesAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            IEnumerable<Task<int>> downloadTasksQuery =
                from url in _urlList
                select ProcessUrlAsync(url, _client);

            Task<int>[] downloadTasks = downloadTasksQuery.ToArray();

            int[] lengths = Task.WhenAll(downloadTasks);
            int total = lengths.Sum();

            await Dispatcher.BeginInvoke(() =>
            {
                stopwatch.Stop();

                _resultsTextBox.Text += $"\nTotal bytes returned:  {total:#,#}";
                _resultsTextBox.Text += $"\nElapsed time:          {stopwatch.Elapsed}\n";
            });
        }

        private async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            byte[] byteArray = await client.GetByteArrayAsync(url);
            await DisplayResultsAsync(url, byteArray);

            return byteArray.Length;
        }

        private Task DisplayResultsAsync(string url, byte[] content) =>
            Dispatcher.BeginInvoke(() =>
                _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}\n")
                      .Task;

        protected override void OnClosed(EventArgs e) => _client.Dispose();
    }
}
