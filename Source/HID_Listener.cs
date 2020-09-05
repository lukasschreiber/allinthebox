using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Timer = System.Threading.Timer;

namespace allinthebox
{
    internal class HID_Listener
    {
        private string barcode_str;
        private readonly List<string> barcodes = new List<string>();
        private int key_pressed, enter_pressed;
        private Main m;
        private readonly XmlDocument settings = new XmlDocument();

        public void normalize_listener()
        {
            key_pressed = 0;
            enter_pressed = 0;
            barcode_str = "";
        }

        public void barcode_listener(object sender, KeyEventArgs e, Main main)
        {
            m = main;
            settings.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));

            key_pressed++;
            var kc = new KeysConverter();

            //Shift in Spanish, French

            if (kc.ConvertToString(e.KeyData).Contains("Umschalttaste") && kc.ConvertToString(e.KeyData).Contains("+"))
            {
                var character = '+';
                var barcodeChar = kc.ConvertToString(e.KeyData).Split(character);
                var charPosition = barcodeChar.Length - 1;
                barcode_str += barcodeChar[charPosition];
            }
            else if (kc.ConvertToString(e.KeyData).Contains("Enter"))
            {
                barcode_str.Replace("Enter", "");
            }
            else if (kc.ConvertToString(e.KeyData).Contains("ControlKey"))
            {
                barcode_str.Replace("ControlKey", "");
            }
            else if (kc.ConvertToString(e.KeyData).Contains("Shift") && kc.ConvertToString(e.KeyData).Contains("+"))
            {
                var character = '+';
                var barcodeChar = kc.ConvertToString(e.KeyData).Split(character);
                var charPosition = barcodeChar.Length - 1;
                barcode_str += barcodeChar[charPosition];
            }
            else if (!IsDigitsOnly(kc.ConvertToString(e.KeyData)) && e.KeyCode != Keys.ShiftKey &&
                     e.KeyCode != Keys.Space && e.KeyCode != Keys.F12 && e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)
            {
                var BarcodeChar = kc.ConvertToString(e.KeyData);
                barcode_str += BarcodeChar;
            }
            else if (e.KeyCode == Keys.Space)
            {
                barcode_str += " ";
            }
            else if (IsDigitsOnly(kc.ConvertToString(e.KeyData)))
            {
                var BarcodeNumeric = kc.ConvertToString(e.KeyData);
                barcode_str += BarcodeNumeric;
            }

            //F12 suffix  
            if (kc.ConvertFromString(settings.SelectSingleNode("/settings/scannerSuffix").InnerXml).ToString()
                .Contains(e.KeyData.ToString())) enter_pressed++;

            Timer timer = null;
            timer = new Timer(obj =>
            {
                key_pressed = 0;
                enter_pressed = 0;
                barcode_str = "";
                timer.Dispose();
            }, null, 100, Timeout.Infinite); //7800

            if (enter_pressed > 0) add_keyHit(main);
        }

        private void loadToList()
        {
            var doc = new XmlDocument();
            doc = m.loadDataBase();
            barcodes.Clear();

            foreach (XmlNode dm in doc.SelectNodes("/data/item"))
            {
                string[] item =
                {
                    dm.SelectSingleNode("barcode").InnerXml
                };
                barcodes.Add(item[0]);
            }
        }

        private void add_keyHit(Main main)
        {
            var minKeyPress = int.Parse(settings.SelectSingleNode("/settings/scannerMinKeyPress").InnerXml); //5
            var minEnterPress = int.Parse(settings.SelectSingleNode("/settings/scannerMinEnterPress").InnerXml); //1
            if (key_pressed > minKeyPress && enter_pressed == minEnterPress)
            {
                loadToList();
                if (barcodes.Contains(barcode_str))
                {
                    //is in db if seleected => handle
                    main.select(barcode_str);
                    normalize_listener();
                }
                else
                {
                    var addView = new add_view(main, barcode_str);
                    addView.Show();
                    addView.item_name.Focus();
                    normalize_listener();
                }
            }
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (var c in str)
                if (c < '0' || c > '9')
                    return false;

            return true;
        }
    }
}