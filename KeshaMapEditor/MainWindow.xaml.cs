using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.IO;

namespace KeshaMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        ObservableCollection<TileTemplateBinding> tilesCollection = new ObservableCollection<TileTemplateBinding>();
        List<TileImage> tiles = new List<TileImage>();
        int tileSize;
        //Image selectionTool;
        bool deletingTile = false;
        bool selectingTile = false;
        int mapWidth = 0;
        int mapHeight = 0;

        ObservableCollection<RecentTileTemplateBinding> recentTiles = new ObservableCollection<RecentTileTemplateBinding>();

        public MainWindow()
        {
            InitializeComponent();

            openFileDialog.Filter = "All Files|*.*";
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.Filter = "JSON files (.json)|*.json";

            //selectionTool = new Image();
            //selectionTool.Width = 34;
            //selectionTool.Height = 35;
            //BitmapImage tempBitmap = new BitmapImage();
            //tempBitmap.BeginInit();
            //tempBitmap.UriSource = new Uri("selectionTool.png", UriKind.Relative);
            //tempBitmap.DecodePixelWidth = 34;
            //tempBitmap.EndInit();
            //selectionTool.Source = tempBitmap;
            //tileCanvas.Children.Add(selectionTool);
            tileCanvas.Width = 0;
            tileCanvas.Height = 0;
        }

        private void addTileButton_Click(object sender, RoutedEventArgs e)
        {
            if (tileSizeTextBox.Text == "")
            {
                MessageBox.Show("You need to enter a tile size first");
                return;
            }
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                if (int.TryParse(tileSizeTextBox.Text, out tileSize))
                {
                    string filename = openFileDialog.FileName;
                    recentTiles.Add(new RecentTileTemplateBinding(filename));
                }
                else
                {
                    MessageBox.Show("The Tize size needs to be a number");
                }
            }
        }

        private void deleteTileButton_Click(object sender, RoutedEventArgs e)
        {
            if (tileListView.SelectedIndex != -1)
            {
                deletingTile = true;
                tileCanvas.Children.RemoveAt(tileListView.SelectedIndex);
                tiles.RemoveAt(tileListView.SelectedIndex);
                tilesCollection.RemoveAt(tileListView.SelectedIndex);
            }
        }

        private void tileListView_Loaded(object sender, RoutedEventArgs e)
        {
            tileListView.ItemsSource = tilesCollection;
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (mapWidthTextBox.Text != "" && mapHeightTextBox.Text != "")
            {
                mapWidth = int.Parse(mapWidthTextBox.Text);
                mapHeight = int.Parse(mapHeightTextBox.Text);
                tileCanvas.Width = mapWidth * tileSize;
                tileCanvas.Height = mapHeight * tileSize;
            }
            for (int i = 0; i < tilesCollection.Count; i++)
            {
                tilesCollection[i].Refresh();
                tiles[i].x = tilesCollection[i].internalX;
                tiles[i].y = tilesCollection[i].internalY;
                Canvas.SetTop(tiles[i].image, tiles[i].y);
                Canvas.SetLeft(tiles[i].image, tiles[i].x);
            }
            tileCanvas.UpdateLayout();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                JObject mapO = new JObject();
                mapO["tile_count"] = tilesCollection.Count;
                mapO["tile_size"] = tileSize;
                JArray tilesA = new JArray();
                foreach (TileTemplateBinding tile in tilesCollection)
                {
                    JObject tileO = new JObject();
                    string[] splitPath = tile.Image.ToString().Split('/');
                    string[] secondSplit = splitPath[splitPath.Length - 1].Split('.');
                    tileO["texture"] = secondSplit[0];
                    tileO["pos_x"] = tile.X;
                    tileO["pos_y"] = tile.Y;
                    tileO["collide"] = tile.Collide;
                    tilesA.Add(tileO);
                }
                mapO["tiles"] = tilesA;
                string filename = saveFileDialog.FileName;
                FileStream file = File.Create(filename);
                StreamWriter streamWriter = new StreamWriter(file);
                streamWriter.Write(mapO.ToString());
                streamWriter.Dispose();
                MessageBox.Show("Map Saved!");
            }
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                tilesCollection.Clear();
                tiles = new List<TileImage>();
                tileCanvas.Children.Clear();
                FileStream file = File.Open(openFileDialog.FileName, FileMode.Open);
                StreamReader streamReader = new StreamReader(file);
                JObject mapO = JObject.Parse(streamReader.ReadToEnd());
                tileSize = (int)mapO["tile_size"];
                tileSizeTextBox.Text = ((int)mapO["tile_size"]).ToString();
                JArray tilesA = (JArray)mapO["tiles"];
                foreach (JObject o in tilesA)
                {
                    string filename = tileLocationButton.Content.ToString();
                    filename += (string)o["texture"] + ".png";
                    tilesCollection.Add(new TileTemplateBinding(filename, tileSize, (int)o["pos_x"], (int)o["pos_y"], (bool)o["collide"]));
                    Image tempImage = new Image();
                    tempImage.Width = tileSize;
                    tempImage.Height = tileSize;
                    BitmapImage tempBitmap = new BitmapImage();
                    tempBitmap.BeginInit();
                    tempBitmap.UriSource = tilesCollection[tilesCollection.Count - 1].Image;
                    tempBitmap.DecodePixelWidth = tileSize;
                    tempBitmap.EndInit();
                    tempImage.Source = tempBitmap;
                    tiles.Add(new TileImage(tempImage, tilesCollection[tilesCollection.Count - 1].internalX, tilesCollection[tilesCollection.Count - 1].internalY));
                    Canvas.SetTop(tiles[tiles.Count - 1].image, tiles[tiles.Count - 1].y);
                    Canvas.SetLeft(tiles[tiles.Count - 1].image, tiles[tiles.Count - 1].x);
                    tileCanvas.Children.Add(tiles[tiles.Count - 1].image);
                }
            }
        }

        private void recentTilesListView_Loaded(object sender, RoutedEventArgs e)
        {
            recentTilesListView.ItemsSource = recentTiles;
        }

        private void addTileButton1_Click(object sender, RoutedEventArgs e)
        {
            if (tileSizeTextBox.Text != "")
            {
                if (int.TryParse(tileSizeTextBox.Text, out tileSize))
                {
                    RecentTileTemplateBinding selectedTile = (RecentTileTemplateBinding)recentTilesListView.SelectedItem;
                    tilesCollection.Add(new TileTemplateBinding(selectedTile.Image.OriginalString, tileSize));
                    Image tempImage = new Image();
                    tempImage.Width = tileSize;
                    tempImage.Height = tileSize;
                    BitmapImage tempBitmap = new BitmapImage();
                    tempBitmap.BeginInit();
                    tempBitmap.UriSource = tilesCollection[tilesCollection.Count - 1].Image;
                    tempBitmap.DecodePixelWidth = tileSize;
                    tempBitmap.EndInit();
                    tempImage.Source = tempBitmap;
                    tiles.Add(new TileImage(tempImage, tilesCollection[tilesCollection.Count - 1].internalX, tilesCollection[tilesCollection.Count - 1].internalY));
                    Canvas.SetTop(tiles[tiles.Count - 1].image, tiles[tiles.Count - 1].y);
                    Canvas.SetLeft(tiles[tiles.Count - 1].image, tiles[tiles.Count - 1].x);
                    tileCanvas.Children.Add(tiles[tiles.Count - 1].image);
                }
                else
                {
                    MessageBox.Show("The Tize size needs to be a number");
                }
            }
            else
            {
                MessageBox.Show("You need to enter a tile size first");
            }
        }

        private void tileLocationButton_Click(object sender, RoutedEventArgs e)
        {
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string tempStr = openFileDialog.FileName;
                string[] splitString = tempStr.Split('\\');
                tileLocationButton.Content = tempStr.Remove(tempStr.Length - 1 - splitString[splitString.Length - 1].Length);
                tileLocationButton.Content += "\\";
            }
        }

        private void tileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!deletingTile && !selectingTile)
            {
                TileTemplateBinding tile = (TileTemplateBinding)tileListView.SelectedItem;
                //tileCanvas.Children.Remove(selectionTool);
                //Canvas.SetTop(selectionTool, tile.internalY + tileSize / 2 - 17);
                //Canvas.SetLeft(selectionTool, tile.internalX + tileSize / 2 - 17);
                //tileCanvas.Children.Add(selectionTool);
            }
            else
            {
                deletingTile = false;
                selectingTile = false;
            }
        }

        private void tileCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (recentTilesListView.Items.Count > 0)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    RecentTileTemplateBinding selectedTile = (RecentTileTemplateBinding)recentTilesListView.SelectedItem;
                    if (selectedTile != null)
                    {
                        if (TileCollision((int)e.GetPosition(tileCanvas).X / tileSize, (int)e.GetPosition(tileCanvas).Y / tileSize))
                        {
                            int tci = GetTileIndexByPosition((int)e.GetPosition(tileCanvas).X / tileSize, (int)e.GetPosition(tileCanvas).Y / tileSize);
                            deletingTile = true;
                            tileCanvas.Children.RemoveAt(tci);
                            tiles.RemoveAt(tci);
                            tilesCollection.RemoveAt(tci);
                        }
                        tilesCollection.Add(new TileTemplateBinding(selectedTile.Image.OriginalString, tileSize, (int)e.GetPosition(tileCanvas).X / tileSize, (int)e.GetPosition(tileCanvas).Y / tileSize, false));
                        Image tempImage = new Image();
                        tempImage.Width = tileSize;
                        tempImage.Height = tileSize;
                        BitmapImage tempBitmap = new BitmapImage();
                        tempBitmap.BeginInit();
                        tempBitmap.UriSource = tilesCollection[tilesCollection.Count - 1].Image;
                        tempBitmap.DecodePixelWidth = tileSize;
                        tempBitmap.EndInit();
                        tempImage.Source = tempBitmap;
                        tiles.Add(new TileImage(tempImage, tilesCollection[tilesCollection.Count - 1].internalX, tilesCollection[tilesCollection.Count - 1].internalY));
                        Canvas.SetTop(tiles[tiles.Count - 1].image, tiles[tiles.Count - 1].y);
                        Canvas.SetLeft(tiles[tiles.Count - 1].image, tiles[tiles.Count - 1].x);
                        tileCanvas.Children.Add(tiles[tiles.Count - 1].image);
                    }
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    selectingTile = true;
                    //tileCanvas.Children.Remove(selectionTool);
                    //Canvas.SetTop(selectionTool, ((int)e.GetPosition(tileCanvas).Y / tileSize) * tileSize  + tileSize / 2 - 17);
                    //Canvas.SetLeft(selectionTool, ((int)e.GetPosition(tileCanvas).X / tileSize) * tileSize + tileSize / 2 - 17);
                    //tileCanvas.Children.Add(selectionTool);
                    tileListView.SelectedIndex = GetTileIndexByPosition((int)e.GetPosition(tileCanvas).X / tileSize, (int)e.GetPosition(tileCanvas).Y / tileSize);
                    tileListView.ScrollIntoView(tileListView.SelectedItem);
                }
            }
        }

        bool TileCollision(int x, int y)
        {
            foreach (TileTemplateBinding tile in tilesCollection)
            {
                if (tile.X == x && tile.Y == y)
                {
                    return true;
                }
            }

            return false;
        }

        int GetTileIndexByPosition(int x, int y)
        {
            for (int i = 0; i < tilesCollection.Count; i++)
            {
                if (tilesCollection[i].X == x && tilesCollection[i].Y == y)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
