using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _9gag
{
    public partial class MainWindow : Window
    {
        public static int count = 0, pre,p=1; public static string loc = "e:\\9gag\\",add="http://www.9gag.com";
        public MainWindow()
        {
            InitializeComponent();
            DirectoryInfo dir = Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\9gag");
            loc = "C:\\Users\\" + Environment.UserName + "\\9gag\\";
            NetworkChange.NetworkAvailabilityChanged +=new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == false)
            {
                string sMessageBoxText = "Internet Connection terminated unexpectedly";
                string sCaption = "9gag Images";
                MessageBoxButton btnMessageBox = MessageBoxButton.OK;
                MessageBoxImage mi = MessageBoxImage.Error;
                MessageBoxResult rs = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, mi);
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void bt1_Click(object sender, RoutedEventArgs e)
        {
            bt2.IsEnabled = false;
            bt3.IsEnabled = true;
            pre = (int)dup1.Value;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            bt1.IsEnabled = false;
            i1.Maximum = (double)(pre);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                count = 0;
                UpdateProgressDelegate update1 = new UpdateProgressDelegate(gifanimplay);
                //UpdateProgressDelegate update2 = new UpdateProgressDelegate(ExitEventHandler);
                System.Windows.Threading.Dispatcher pdDispatcher = i1.Dispatcher;
                //StreamWriter sw = new StreamWriter("E:\\9gag\\link.txt", true);
                string doc = "";
            h:
                WebClient client = new WebClient();
                doc = client.DownloadString(add);
                //sw.Write(doc.ToString());
                //sw.Flush();
                String[] rt = new String[2];
                String[] golo = new String[20];
                golo = Regex.Split(doc, "src=\"");
                rt = Regex.Split(doc, "class=\"btn badge-load-more-post\" href=\"");
                String h = rt[1];
                rt = Regex.Split(h, "\" data-loading-text");
                rt[0] = add + rt[0];
                for (int x = 2; x < 14; x++)
                {
                    if (count < pre)
                    {
                        if (p == 1)
                        {
                            
                            String[] gol = new String[3];
                            String[] l = new String[3];
                           // sw.Write(x.ToString() + "     " + golo[x] + "\r\n");
                            //sw.Flush();
                            gol = Regex.Split(golo[x], "\" alt=");
                            gol[0].TrimStart();

                            string ch = gol[0].Substring(0, 1);
                            string ch2 = gol[0].Substring(gol[0].Length - 1);
                            if (ch == "h" || ch == "H")
                            {
                                MessageBox.Show(ch2+"       "+gol[0]);
                                gol[1] = gol[1].Substring(1);
                                l = Regex.Split(gol[1], "\"/");
                                StringWriter l1 = new StringWriter();
                                HttpUtility.HtmlDecode(l[0], l1);
                                string l2 = Regex.Replace(l1.ToString(), "[@,\\:\"?*;.\\\\]", "");
                               //sw.Write(x.ToString() + "     " + gol[0] + "\r\n");
                                //sw.Flush();
                                using (WebClient wc = new WebClient())
                                {
                                    byte[] fileBytes = wc.DownloadData(gol[0]);
                                    string fileType = wc.ResponseHeaders[HttpResponseHeader.ContentType];
                                    string sa = "";
                                    if (fileType != null)
                                    {
                                        switch (fileType)
                                        {
                                            case "image/jpeg":
                                                sa += ".jpg";
                                                break;
                                            case "image/gif":
                                                sa += ".gif";
                                                break;
                                            case "image/png":
                                                sa += ".png";
                                                break;
                                            default:
                                                break;
                                        }
                                        bool b = File.Exists(loc + l2.ToString() + ".jpg");
                                        if (b == true)
                                        {
                                            File.Delete(loc + l2.ToString() + ".jpg");
                                            File.WriteAllBytes(loc + l2.ToString() + sa, fileBytes);
                                           
                                        }
                                        else
                                        {
                                            count++;
                                            File.WriteAllBytes(loc + l2.ToString() + sa, fileBytes);
                                            pdDispatcher.BeginInvoke(update1, count);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {  p = 1; }
                    }
                    else
                    { goto exit;  }
                }
                add = rt[0];
                goto h;
            exit:
                //sw.Close();
                string sMessageBoxText = "Image fetching complete";
                string sCaption = "9gag Images";
                MessageBoxButton btnMessageBox = MessageBoxButton.OK;
                MessageBoxImage mi = MessageBoxImage.Information;
                MessageBoxResult rs = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, mi);
                //pdDispatcher.BeginInvoke(update2, count);      
            }
            catch (WebException ex)
            {
                string sMessageBoxText = "No Internet Connection Available";
                string sCaption = "9gag Images";
                MessageBoxButton btnMessageBox = MessageBoxButton.OK;
                MessageBoxImage mi = MessageBoxImage.Error;
                MessageBoxResult rs = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, mi);
            }                    
            catch (Exception ex)
            {
                string sMessageBoxText = ex.ToString();
                string sCaption = "9gag Images";
                MessageBoxButton btnMessageBox = MessageBoxButton.OK;
                MessageBoxImage mi = MessageBoxImage.Error;
                MessageBoxResult rs = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, mi);
            }       
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bt1.IsEnabled = true;
            bt2.IsEnabled = true;
            gifanimpause();
        }
        public delegate void UpdateProgressDelegate(int c);

        public void gifanimplay(int c)
        {
            //c++;
            i1.Value++;
            lb2.Content = "Loaded Image no. " + c.ToString();
        }
        public void gifanimpause()
        {

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog sv = new VistaFolderBrowserDialog();
            sv.ShowDialog();
            loc = Regex.Replace(sv.SelectedPath.ToString(), @"\\", @"\\");
            loc = loc + "\\\\";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            add = "http://www.9gag.com/trending"; p = 1;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            add = "http://www.9gag.com/hot";
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            add = "http://www.9gag.com/gif"; p = 1;
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            add = "http://www.9gag.com/fresh"; p = 1;
        }

        /*private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            add = "http://www.9gag.com/nsfw"; p = 1;
        }                                         */

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            add = "http://www.9gag.com/cute"; p = 1;
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            add = "http://www.9gag.com/geeky"; p = 1;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bt3_Click(object sender, RoutedEventArgs e)
        {
            lb2.Content = "Process Aborted";
            bt3.IsEnabled = false;
            count = pre + 1;
            i1.Value = 0;
        } 
    }
}
