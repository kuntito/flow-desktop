using flow_desktop.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ABI.Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// TODO add debounce to search
namespace flow_desktop.UI.Components
{
    // TODO add play next, play later
    //  add local listen count db
    public sealed partial class SearchBar : UserControl
    {
        private static readonly DependencyProperty dependencyPropOnSearch =
            DependencyProperty.Register(
                nameof(OnSearch),
                typeof(Action<string>),
                typeof(SearchBar),
                new PropertyMetadata(null)
            );

        public Action<string> OnSearch
        {
            get => (Action<string>)GetValue(dependencyPropOnSearch);
            set => SetValue(dependencyPropOnSearch, value);
        }

        private DispatcherTimer? _debounceTimer;
        private void OnQueryChange(object sender, TextChangedEventArgs e)
        {
            var query = ((TextBox)sender).Text;
            
            _debounceTimer?.Stop();
            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _debounceTimer.Tick += (s, args) =>
            {
                _debounceTimer.Stop();
                OnSearch?.Invoke(query);
            };
            _debounceTimer.Start();
        }
        
        private static readonly DependencyProperty dependencyPropSongSearchState =
            DependencyProperty.Register(
                nameof(dependencyWrapperSongSearchState),
                typeof(SongSearchState),
                typeof(SearchBar),
                new PropertyMetadata(
                    null, 
                    OnSearchStateChanged
                )
            );

        public SongSearchState dependencyWrapperSongSearchState
        {
            get => (SongSearchState)GetValue(dependencyPropSongSearchState);
            set => SetValue(dependencyPropSongSearchState, value);
        }
        
        private static void OnSearchStateChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e 
        )
        {
            var searchBar = (SearchBar)d;
            searchBar.UpdateSearchStateVisuals();
        }

        private void UpdateSearchStateVisuals()
        {
            SongSearchResultsPopup.IsOpen = dependencyWrapperSongSearchState is not SongSearchState.Idle;

            switch (dependencyWrapperSongSearchState)
            {
                case SongSearchState.Searching:
                    SongSearchPopupContent.Content = new ProgressRing
                    {
                        IsActive = true
                    };
                    break;
                case SongSearchState.NoResults:
                    SongSearchPopupContent.Content = new TextBlock
                    {
                        Text = "couldn't find it.",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    break;
                case SongSearchState.FoundResults found:
                    var listView = CreateSongSearchResultsListView();
                    listView.ItemsSource = found.SongSearchResults;
                    SongSearchPopupContent.Content = listView;
                    break;
                case SongSearchState.Error:
                    // TODO snackbar instead...
                    SongSearchPopupContent.Content = new TextBlock
                    {
                        Text = "sumn' wrong, i hold my head.",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    break;
                default:
                    SongSearchPopupContent.Content = null;
                    break;
            }
        }

        private ListView CreateSongSearchResultsListView()
        {
            var listView = new ListView
            {
                Padding = new Thickness(0, 0, 0, 8),
                ItemTemplate = (DataTemplate)Resources["SongSearchSLITemplate"],
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            var itemStyle = new Style(typeof(ListViewItem));
            itemStyle.Setters.Add(new Setter(MarginProperty, new Thickness(8, 0, 0, 8)));
            itemStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            listView.ItemContainerStyle = itemStyle;

            return listView;
        }
            
            
        public SearchBar()
        {
            InitializeComponent();
        }
    }
}
