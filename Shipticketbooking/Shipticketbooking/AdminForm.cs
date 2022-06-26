using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Shipticketbooking 
{
    public partial class AdminForm : Form
    {
        Database database = new Database();
        SQLiteCommand command;
        DataTable dataTable;
        DataTable dataTable2;
        SQLiteDataAdapter adapter;

        double reservedCabinsSumByTravelType;
        int reservedCabinsSumWithWindow;
        int reservedCabinsSumWithoutWindow;
        int reservedCabinsCountByName;
        double reservedCabinsSumByName;
        int reservedCabinsSumBySightSeeing;

        string travelType = "";
        int[] sightSeeingInHarbors = new int[4];
        string[] harbors = new string[4];
        string sightSeeing = "";

        public AdminForm()
        {
            InitializeComponent();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {

        }

        //Alkalmazás bezárása
        private void AdminForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        //Alkalmazás bezárása, bejelentkezési lap megnyitása.
        private void exitButton_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            this.Hide();
            form1.Show();

        }

        //Foglaltsági adatok lekérdezése az adatbázisból és kabinok megjelenítése a TableLayoutPanel-ben.
        //Ha az adott kabin foglalt, akkor a TableLayoutPanel cellája piros, különben zöld.
        private void showCabins(int selectedRegionIndex, int selectedTravelIndex)
        {
            if (selectedRegionIndex == 0 && selectedTravelIndex == 0)
            {
                command = new SQLiteCommand("select * from Travel1 join Traveltype on Travel1.traveltypeid = Traveltype.id",database.getConnection());
                travelType = "Travel1";
            }
            if (selectedRegionIndex == 0 && selectedTravelIndex == 1)
            {
                command = new SQLiteCommand("select * from Travel2 join Traveltype on Travel2.traveltypeid = Traveltype.id", database.getConnection());
                travelType = "Travel2";
            }
            if (selectedRegionIndex == 1 && selectedTravelIndex == 0)
            {
                command = new SQLiteCommand("select * from Travel3 join Traveltype on Travel3.traveltypeid = Traveltype.id", database.getConnection());
                travelType = "Travel3";
            }
            if (selectedRegionIndex == 1 && selectedTravelIndex == 1)
            {
                command = new SQLiteCommand("select * from Travel4 join Traveltype on Travel4.traveltypeid = Traveltype.id", database.getConnection());
                travelType = "Travel4";
            }

            adapter = new SQLiteDataAdapter(command);
            dataTable = new DataTable();
            adapter.Fill(dataTable);
            travelsDataGridView.DataSource = dataTable;

            harbors[0] = dataTable.Rows[0][13].ToString();
            harbors[1] = dataTable.Rows[0][14].ToString();
            harbors[2] = dataTable.Rows[0][15].ToString();
            harbors[3] = dataTable.Rows[0][16].ToString();

            for (int i = 0; i <= 7; i++)
            {
                if ((Convert.ToInt32(dataTable.Rows[i][2]) == 1))
                {
                    leftSideWithWindowTableLayoutPanel.Controls[i].BackColor = Color.Red;
                }
                else
                {
                    leftSideWithWindowTableLayoutPanel.Controls[i].BackColor = Color.Green;
                }

                if ((Convert.ToInt32(dataTable.Rows[i + 8][2]) == 1))
                {
                    leftSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Red;

                }
                else
                {
                    leftSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Green;
                }

                if ((Convert.ToInt32(dataTable.Rows[i + 16][2]) == 1))
                {
                    rightSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Red;

                }
                else
                {
                    rightSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Green;
                }

                if ((Convert.ToInt32(dataTable.Rows[i + 24][2]) == 1))
                {
                    rightSideWithWindowTableLayoutPanel.Controls[i].BackColor = Color.Red;

                }
                else
                {
                    rightSideWithWindowTableLayoutPanel.Controls[i].BackColor = Color.Green;
                }
            }
        }

        //A hajóút kiválasztására szolgáló comboBox feltöltése aszerint, hogy melyik tájegység lett kiválasztva.
        private void selectRegionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectRegionComboBox.SelectedIndex == 0)
            {
                selectTravelComboBox.Items.Clear();
                selectTravelComboBox.Text = "";
                selectTravelComboBox.Items.Add("Róma-Tunisz-Barcelona-Marseille-Róma");
                selectTravelComboBox.Items.Add("Athén-Isztambul-Libanon-Alexandria-Athén");
            }
            if (selectRegionComboBox.SelectedIndex == 1)
            {
                selectTravelComboBox.Items.Clear();
                selectTravelComboBox.Text = "";
                selectTravelComboBox.Items.Add("Stockholm - Helsinki - Szentpétervár - Tallinn - Stockholm");
                selectTravelComboBox.Items.Add("Koppenhága - Göteborg - Oslo - Edinburgh - Koppenhága");
            }
        }

        //A kabinok megjelenítése a függvény meghívásával. Paraméterben megkapja a kiválasztott tájegység és a kiválasztott
        //hajóút indexét.
        private void selectTravelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showCabins(selectRegionComboBox.SelectedIndex, selectTravelComboBox.SelectedIndex);
            allNameFromDatabase();

            searchByNameLabel.Text = "";
        }

        //Összes név lekérdezése az adatbázisból a kiválasztott uticélra
        //Bevételt számító függvény meghívása a nevekkel
        //Bevételek megjelenítése
        private void allNameFromDatabase()
        {
            reservedCabinsSumByTravelType = 0;
            reservedCabinsSumWithWindow = 0;
            reservedCabinsSumWithoutWindow = 0;
            reservedCabinsSumBySightSeeing = 0;

            command = new SQLiteCommand("select distinct firstname, lastname from '"+travelType+"' where reserved = 1",database.getConnection());

            adapter = new SQLiteDataAdapter(command);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            for(int i = 0; i < dataTable.Rows.Count; i++)
            {
                allPriceSum(dataTable.Rows[i][0].ToString(),dataTable.Rows[i][1].ToString());
            }

            allPriceSumLabel.Text = reservedCabinsSumByTravelType.ToString() + " Ft";
            priceSumWithWindowLabel.Text = reservedCabinsSumWithWindow.ToString() + " Ft";
            priceSumWithoutWindowLabel.Text = reservedCabinsSumWithoutWindow.ToString() + " Ft";
            priceSumBySightSeeingLabel.Text = reservedCabinsSumBySightSeeing.ToString() + " Ft";
        }

        //Foglalási adatok név szerint
        //Foglaló neve, kabinok száma és összege, felhasznált kupon, városnézések
        private void searchByNameButton_Click(object sender, EventArgs e)
        {
            reservedCabinsCountByName = 0;
            reservedCabinsSumByName = 0;
            sightSeeing = "";

            if(firstNameTextBox.Text.Length > 0 && lastNameTextBox.Text.Length > 0 && travelType.Length > 0)
            {
                command = new SQLiteCommand("select cabintype, reserved, coupon, " +
                    "sightseeinginharbor1, sightseeinginharbor2, " +
                    "sightseeinginharbor3, sightseeinginharbor4 from '" + travelType + "' where firstname='" + firstNameTextBox.Text + "' " +
                                            "and lastname = '" + lastNameTextBox.Text + "'", database.getConnection());

                adapter = new SQLiteDataAdapter(command);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                if(dataTable.Rows.Count >= 1)
                {
                    sightSeeingInHarbors[0] = Convert.ToInt32(dataTable.Rows[0][3]);
                    sightSeeingInHarbors[1] = Convert.ToInt32(dataTable.Rows[0][4]);
                    sightSeeingInHarbors[2] = Convert.ToInt32(dataTable.Rows[0][5]);
                    sightSeeingInHarbors[3] = Convert.ToInt32(dataTable.Rows[0][6]);

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (dataTable.Rows[i][0].ToString() == "ablakos" && Convert.ToInt32(dataTable.Rows[i][1]) == 1)
                        {
                            reservedCabinsCountByName++;
                            reservedCabinsSumByName += 30000;
                        }
                        if (dataTable.Rows[i][0].ToString() == "belső" && Convert.ToInt32(dataTable.Rows[i][1]) == 1)
                        {
                            reservedCabinsCountByName++;
                            reservedCabinsSumByName += 20000;
                        }
                    }

                    for (int i = 0; i <= 3; i++)
                    {
                        if (sightSeeingInHarbors[i] == 1)
                        {
                            sightSeeing += harbors[i] + ", ";
                            reservedCabinsSumByName += 10000 * reservedCabinsCountByName;
                        }
                    }

                    if(dataTable.Rows[0][2].ToString() == "H10")
                    {
                        reservedCabinsSumByName = reservedCabinsSumByName - (reservedCabinsSumByName * 0.1);
                    }
                    if (dataTable.Rows[0][2].ToString() == "H5")
                    {
                        reservedCabinsSumByName = reservedCabinsSumByName - (reservedCabinsSumByName * 0.05);
                    }


                    searchByNameLabel.Text = lastNameTextBox.Text + " " + firstNameTextBox.Text + " lefoglalt " + reservedCabinsCountByName + "" +
                        " db kabint. " + "\nA lefoglalt kabin(ok) ára: " + reservedCabinsSumByName + " Ft\n" +
                        "A felhasznált kupon: " + dataTable.Rows[0][2] + "\nVárosnézés: " + sightSeeing;

                    firstNameTextBox.Text = "";
                    lastNameTextBox.Text = "";
                }
                else
                {
                    MessageBox.Show("A megadott név nem szerepel az adatbázisban.");
                    firstNameTextBox.Text = "";
                    lastNameTextBox.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Válassz uticélt és add meg a vezetéknevet, keresztnevet.");
                firstNameTextBox.Text = "";
                lastNameTextBox.Text = "";
            }



        }

        //Info menüpont, program készítője.
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A programot készítette: Tóth János");
        }

        //Bevétel kiszámítása az ablakos kabinokra
        //Bevétel kiszámítása a belső kabinokra
        //Bevétel kiszámítása a városnézésekből
        //Teljes bevétel kiszámítása kedvezményekkel
        private void allPriceSum(string firstName, string lastName)
        {
            int count = 0;
            double sum = 0;
            int[] sightSeeing = new int[4];

            command = new SQLiteCommand("select cabintype, coupon, sightseeinginharbor1, " +
                "sightseeinginharbor2, sightseeinginharbor3, sightseeinginharbor4 from '" + travelType + "' where firstname='" + firstName + "' " +
                            "and lastname = '" + lastName + "'", database.getConnection());

            adapter = new SQLiteDataAdapter(command);
            dataTable2 = new DataTable();
            adapter.Fill(dataTable2);

            sightSeeing[0] = Convert.ToInt32(dataTable2.Rows[0][2]);
            sightSeeing[1] = Convert.ToInt32(dataTable2.Rows[0][3]);
            sightSeeing[2] = Convert.ToInt32(dataTable2.Rows[0][4]);
            sightSeeing[3] = Convert.ToInt32(dataTable2.Rows[0][5]);

            for (int i = 0; i < dataTable2.Rows.Count; i++)
            {
                if (dataTable2.Rows[i][0].ToString() == "ablakos")
                {
                    sum += 30000;
                    reservedCabinsSumWithWindow += 30000;
                    count++;
                }
                if (dataTable2.Rows[i][0].ToString() == "belső")
                {
                    sum += 20000;
                    reservedCabinsSumWithoutWindow += 20000;
                    count++;
                }
            }

            for(int i = 0; i <= 3; i++)
            {
                if(sightSeeing[i] == 1)
                {
                    sum += 10000 * count;
                    reservedCabinsSumBySightSeeing += 10000 * count;
                }
            }

            if (dataTable2.Rows[0][1].ToString() == "H10")
            {
                sum = sum - (sum * 0.1);
            }
            if (dataTable2.Rows[0][1].ToString() == "H5")
            {
                sum = sum - (sum * 0.05);
            }

            reservedCabinsSumByTravelType += sum;
        }
    }
}
