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
    public partial class TicketBuyForm : Form
    {
        Database database = new Database();
        SQLiteCommand command;
        DataTable dataTable;
        SQLiteDataAdapter adapter;

        int reservedCabinsCount;
        double reservedCabinsSum;
        string travelType = "";
        int[] sightSeeingInHarbors = new int[4];
        bool reserveSucceed;
        string coupon;

        public TicketBuyForm()
        {
            InitializeComponent();
        }

        private void TicketBuyForm_Load(object sender, EventArgs e)
        {

        }

        //Alkalmazás bezárása
        private void TicketBuyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        //Foglaltsági adatok lekérdezése az adatbázisból és kabinok megjelenítése a TableLayoutPanel-ben.
        //Ha az adott kabin foglalt, akkor a TableLayoutPanel cellája piros, különben zöld.
        //A kiválasztott hajóút alapján a kikötők megjelenítése.
        private void showCabins(int selectedRegionIndex, int selectedTravelIndex)
        {
            if(selectedRegionIndex == 0 && selectedTravelIndex == 0)
            {
                command = new SQLiteCommand("select reserved from Travel1", database.getConnection());
                harbor1CheckBox.Text = "Róma";
                harbor2CheckBox.Text = "Tunisz";
                harbor3CheckBox.Text = "Barcelona";
                harbor4CheckBox.Text = "Marseille";
                travelType = "Travel1";
            }
            if (selectedRegionIndex == 0 && selectedTravelIndex == 1)
            {
                command = new SQLiteCommand("select reserved from Travel2", database.getConnection());
                harbor1CheckBox.Text = "Athén";
                harbor2CheckBox.Text = "Isztambul";
                harbor3CheckBox.Text = "Libanon";
                harbor4CheckBox.Text = "Alexandria";
                travelType = "Travel2";
            }
            if (selectedRegionIndex == 1 && selectedTravelIndex == 0)
            {
                command = new SQLiteCommand("select reserved from Travel3", database.getConnection());
                harbor1CheckBox.Text = "Stockholm";
                harbor2CheckBox.Text = "Helsinki";
                harbor3CheckBox.Text = "Szentpétervár";
                harbor4CheckBox.Text = "Tallinn";
                travelType = "Travel3";
            }
            if (selectedRegionIndex == 1 && selectedTravelIndex == 1)
            {
                command = new SQLiteCommand("select reserved from Travel4", database.getConnection());
                harbor1CheckBox.Text = "Koppenhága";
                harbor2CheckBox.Text = "Göteborg";
                harbor3CheckBox.Text = "Oslo";
                harbor4CheckBox.Text = "Edinburgh";
                travelType = "Travel4";
            }

            adapter = new SQLiteDataAdapter(command);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            for (int i = 0; i <= 7; i++)
            {
                if ((Convert.ToInt32(dataTable.Rows[i][0]) == 1))
                {
                    leftSideWithWindowTableLayoutPanel.Controls[i].BackColor = Color.Red;
                    
                }
                else
                {
                    leftSideWithWindowTableLayoutPanel.Controls[i].BackColor = Color.Green;
                }

                if ((Convert.ToInt32(dataTable.Rows[i+8][0]) == 1))
                {
                    leftSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Red;

                }
                else
                {
                    leftSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Green;
                }

                if ((Convert.ToInt32(dataTable.Rows[i + 16][0]) == 1))
                {
                    rightSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Red;

                }
                else
                {
                    rightSideWithoutWindowTableLayoutPanel.Controls[i].BackColor = Color.Green;
                }

                if ((Convert.ToInt32(dataTable.Rows[i + 24][0]) == 1))
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
            if(selectRegionComboBox.SelectedIndex == 0)
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

        //A kabinok megjelenítése a függvény meghívásával. Paraméterben megkapja a kiválasztott régió és a kiválasztott
        //hajóút indexét.
        private void selectTravelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showCabins(selectRegionComboBox.SelectedIndex , selectTravelComboBox.SelectedIndex);
        }

        //Alkalmazás bezárása, bejelentkezési lap megnyitása.
        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }

        //A lefoglalni kívánt kabinok kiválasztása a bal oldali ablakos kabinok közül és ezek megjelenítése.
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            reserveCabin(leftSideWithWindowcheckedListBox.SelectedIndex, leftSideWithWindowTableLayoutPanel, leftSideWithoutWindowTableLayoutPanel);
        }

        //Foglalás véglegesítése. Adatok küldése az adatbázisnak majd a módosított adatok lekérése és megjelenítése a TableLayoutPanel-ben.
        private void reserveButton_Click(object sender, EventArgs e)
        {
            int databaseIndex = 0;
            reserveSucceed = false;

            showReservationDatasButton_Click(sender, e);

            for (int i = 1; i <= 8; i++)
            {
                if(leftSideWithWindowTableLayoutPanel.Controls[i-1].BackColor == Color.Yellow && firstNameTextBox.Text.Length > 0 && 
                    lastNameTextBox.Text.Length > 0)
                {
                    database.openConnection();
                    command = new SQLiteCommand("update '"+travelType+"' set reserved='"+1+"', firstname='"+firstNameTextBox.Text+"', lastname='"+lastNameTextBox.Text+"'," +
                        "coupon='"+coupon+"',sightseeinginharbor1='"+sightSeeingInHarbors[0]+"'," +
                        "sightseeinginharbor2='" + sightSeeingInHarbors[1]+"'," +
                        "sightseeinginharbor3='" + sightSeeingInHarbors[2] + "'," +
                        "sightseeinginharbor4='" + sightSeeingInHarbors[3] + "' where id='" + i+"'",database.getConnection());
                    command.ExecuteNonQuery();
                    database.closeConnection();
                    reserveSucceed = true;
                }
                if (leftSideWithoutWindowTableLayoutPanel.Controls[i - 1].BackColor == Color.Yellow && firstNameTextBox.Text.Length > 0 &&
                    lastNameTextBox.Text.Length > 0)
                {
                    databaseIndex = i + 8;
                    database.openConnection();
                    command = new SQLiteCommand("update '" + travelType + "' set reserved='" + 1 + "', firstname='" + firstNameTextBox.Text + "', lastname='" + lastNameTextBox.Text + "'," +
                        "coupon='" + coupon + "',sightseeinginharbor1='" + sightSeeingInHarbors[0] + "'," +
                        "sightseeinginharbor2='" + sightSeeingInHarbors[1] + "'," +
                        "sightseeinginharbor3='" + sightSeeingInHarbors[2] + "'," +
                        "sightseeinginharbor4='" + sightSeeingInHarbors[3] + "' where id='" + databaseIndex + "'", database.getConnection());
                    command.ExecuteNonQuery();
                    database.closeConnection();
                    reserveSucceed = true;
                }
                if (rightSideWithoutWindowTableLayoutPanel.Controls[i - 1].BackColor == Color.Yellow && firstNameTextBox.Text.Length > 0 &&
                    lastNameTextBox.Text.Length > 0)
                {
                    databaseIndex = i + 16;
                    database.openConnection();
                    command = new SQLiteCommand("update '" + travelType + "' set reserved='" + 1 + "', firstname='" + firstNameTextBox.Text + "', lastname='" + lastNameTextBox.Text + "'," +
                        "coupon='" + coupon + "',sightseeinginharbor1='" + sightSeeingInHarbors[0] + "'," +
                        "sightseeinginharbor2='" + sightSeeingInHarbors[1] + "'," +
                        "sightseeinginharbor3='" + sightSeeingInHarbors[2] + "'," +
                        "sightseeinginharbor4='" + sightSeeingInHarbors[3] + "' where id='" + databaseIndex + "'", database.getConnection());
                    command.ExecuteNonQuery();
                    database.closeConnection();
                    reserveSucceed = true;
                }
                if (rightSideWithWindowTableLayoutPanel.Controls[i - 1].BackColor == Color.Yellow && firstNameTextBox.Text.Length > 0 &&
                    lastNameTextBox.Text.Length > 0)
                {
                    databaseIndex = i + 24;
                    database.openConnection();
                    command = new SQLiteCommand("update '" + travelType + "' set reserved='" + 1 + "', firstname='" + firstNameTextBox.Text + "', lastname='" + lastNameTextBox.Text + "'," +
                        "coupon='" + coupon + "',sightseeinginharbor1='" + sightSeeingInHarbors[0] + "'," +
                        "sightseeinginharbor2='" + sightSeeingInHarbors[1] + "'," +
                        "sightseeinginharbor3='" + sightSeeingInHarbors[2] + "'," +
                        "sightseeinginharbor4='" + sightSeeingInHarbors[3] + "' where id='" + databaseIndex + "'", database.getConnection());
                    command.ExecuteNonQuery();
                    database.closeConnection();
                    reserveSucceed = true;
                }
            }

            if(reserveSucceed)
            {
                MessageBox.Show("Sikeres foglalás.\n" +
                    lastNameTextBox.Text + " " + firstNameTextBox.Text + " lefoglalt " + reservedCabinsCount + " db kabint.\n" +
                "A lefoglalt kabin(ok) ára: " + reservedCabinsSum + "Ft");

                showCabins(selectRegionComboBox.SelectedIndex, selectTravelComboBox.SelectedIndex);
                clearInputDatas();
            }
            else
            {
                MessageBox.Show("Válassz kabint és add meg a teljes nevedet.");
            }
        }

        //A foglalási adatok megjelenítése. Foglaló neve, foglalt kabinok száma, foglalás teljes ára.
        private void showReservationDatasButton_Click(object sender, EventArgs e)
        {
            reservedCabinsCount = 0;
            reservedCabinsSum = 0;
            sightSeeingInHarbors[0] = 0;
            sightSeeingInHarbors[1] = 0;
            sightSeeingInHarbors[2] = 0;
            sightSeeingInHarbors[3] = 0;
            coupon = "";

            for (int i = 0; i <= 7; i++)
            {
                if (leftSideWithWindowTableLayoutPanel.Controls[i].BackColor == Color.Yellow)
                {
                    reservedCabinsCount++;
                    reservedCabinsSum += 30000;
                }
                if (leftSideWithoutWindowTableLayoutPanel.Controls[i].BackColor == Color.Yellow)
                {
                    reservedCabinsCount++;
                    reservedCabinsSum += 20000;
                }
                if (rightSideWithoutWindowTableLayoutPanel.Controls[i].BackColor == Color.Yellow)
                {
                    reservedCabinsCount++;
                    reservedCabinsSum += 20000;
                }
                if (rightSideWithWindowTableLayoutPanel.Controls[i].BackColor == Color.Yellow)
                {
                    reservedCabinsCount++;
                    reservedCabinsSum += 30000;
                }
            }

            if (harbor1CheckBox.Checked)
            {
                reservedCabinsSum += 10000 * reservedCabinsCount;
                sightSeeingInHarbors[0] = 1;
            }
            if (harbor2CheckBox.Checked)
            {
                reservedCabinsSum += 10000 * reservedCabinsCount;
                sightSeeingInHarbors[1] = 1;
            }
            if (harbor3CheckBox.Checked)
            {
                reservedCabinsSum += 10000 * reservedCabinsCount;
                sightSeeingInHarbors[2] = 1;
            }
            if (harbor4CheckBox.Checked)
            {
                reservedCabinsSum += 10000 * reservedCabinsCount;
                sightSeeingInHarbors[3] = 1;
            }

            if (couponTextBox.Text == "H5")
            {
                reservedCabinsSum = reservedCabinsSum - (reservedCabinsSum * 0.05);
                coupon = "H5";
            }
            if (couponTextBox.Text == "H10")
            {
                reservedCabinsSum = reservedCabinsSum - (reservedCabinsSum * 0.1);
                coupon = "H10";
            }


            reservedCabinsLabel.Text = lastNameTextBox.Text + " " + firstNameTextBox.Text + " lefoglalt " + reservedCabinsCount + " db kabint.\n" +
                "A lefoglalt kabin(ok) ára: " + reservedCabinsSum + " Ft";
        }

        //A lefoglalni kívánt kabinok kiválasztása a bal oldali ablak nélküliek közül és ezek megjelenítése.
        private void leftSideWithoutWindowcheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            reserveCabin(leftSideWithoutWindowcheckedListBox.SelectedIndex, leftSideWithoutWindowTableLayoutPanel, leftSideWithWindowTableLayoutPanel);
        }

        //A lefoglalni kívánt kabinok kiválasztása a jobb oldali ablak nélküliek közül és ezek megjelenítése.
        private void rightSideWithoutWindowcheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            reserveCabin(rightSideWithoutWindowcheckedListBox.SelectedIndex,rightSideWithoutWindowTableLayoutPanel, rightSideWithWindowTableLayoutPanel);
        }

        //A lefoglalni kívánt kabinok kiválasztása a jobb oldali ablakosok közül és ezek megjelenítése.
        private void rightSideWithWindowcheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            reserveCabin(rightSideWithWindowcheckedListBox.SelectedIndex,rightSideWithWindowTableLayoutPanel, rightSideWithoutWindowTableLayoutPanel);
        }

        //Info menüpont, program készítője
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A programot készítette: Tóth János");
        }

        //A lefoglalni kívánt kabinok törlése és minden egyéb beviteli mező törlése.
        private void deleteReservationButton_Click(object sender, EventArgs e)
        {

            if(travelType.Length > 0)
            {
                showCabins(selectRegionComboBox.SelectedIndex, selectTravelComboBox.SelectedIndex);
            }
            clearInputDatas();
        }

        //Input mezők adatainak törlése, checkbox-ok és checklistbox-ok törlése
        //Megjelenített foglalási adatok törlése
        private void clearInputDatas()
        {
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            couponTextBox.Text = "";
            reservedCabinsLabel.Text = "";
            harbor1CheckBox.Checked = false;
            harbor2CheckBox.Checked = false;
            harbor3CheckBox.Checked = false;
            harbor4CheckBox.Checked = false;

            for (int i = 0; i <= 7; i++)
            {
                leftSideWithWindowcheckedListBox.SetItemCheckState(i, CheckState.Unchecked);
                leftSideWithoutWindowcheckedListBox.SetItemCheckState(i, CheckState.Unchecked);
                rightSideWithWindowcheckedListBox.SetItemCheckState(i, CheckState.Unchecked);
                rightSideWithoutWindowcheckedListBox.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        //Kabinok lefoglalása.
        //Csak olyan kabint lehet foglalni ami szabad, és vízszintesen és függőlegesen nincs mellette foglalt kabin.
        //A foglalt kabin piros, a szabad zöld a foglalás alatt lévő pedig sárga színű.
        private void reserveCabin(int index, TableLayoutPanel selectedTableLayoutPanel, TableLayoutPanel nextToTableLayoutPanel)
        {
            if (selectedTableLayoutPanel.Controls[index].BackColor == Color.Green)
            {
                if (index == 0)
                {
                    if ((selectedTableLayoutPanel.Controls[index + 1].BackColor == Color.Green ||
                        selectedTableLayoutPanel.Controls[index + 1].BackColor == Color.Yellow)
                        &&
                        (nextToTableLayoutPanel.Controls[index].BackColor == Color.Green ||
                        nextToTableLayoutPanel.Controls[index].BackColor == Color.Yellow))
                    {
                        selectedTableLayoutPanel.Controls[index].BackColor = Color.Yellow;
                    }
                    else
                    {
                        MessageBox.Show("Ezt a kabint nem lehet lefoglalni a vírus miatt. Válassz egy másikat.");
                    }
                }
                if (index == 7)
                {
                    if ((selectedTableLayoutPanel.Controls[index - 1].BackColor == Color.Green ||
                        selectedTableLayoutPanel.Controls[index - 1].BackColor == Color.Yellow)
                        &&
                        (nextToTableLayoutPanel.Controls[index].BackColor == Color.Green ||
                        nextToTableLayoutPanel.Controls[index].BackColor == Color.Yellow))
                    {
                        selectedTableLayoutPanel.Controls[index].BackColor = Color.Yellow;
                    }
                    else
                    {
                        MessageBox.Show("Ezt a kabint nem lehet lefoglalni a vírus miatt. Válassz egy másikat.");
                    }
                }
                if (index > 0 && index < 7)
                {
                    if ((selectedTableLayoutPanel.Controls[index + 1].BackColor == Color.Green ||
                        selectedTableLayoutPanel.Controls[index + 1].BackColor == Color.Yellow)
                        &&
                        (selectedTableLayoutPanel.Controls[index - 1].BackColor == Color.Green ||
                        selectedTableLayoutPanel.Controls[index - 1].BackColor == Color.Yellow)
                        &&
                        (nextToTableLayoutPanel.Controls[index].BackColor == Color.Green ||
                        nextToTableLayoutPanel.Controls[index].BackColor == Color.Yellow))
                    {
                        selectedTableLayoutPanel.Controls[index].BackColor = Color.Yellow;
                    }
                    else
                    {
                        MessageBox.Show("Ezt a kabint nem lehet lefoglalni a vírus miatt. Válassz egy másikat.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Ez a kabin már foglalt. Válassz egy másikat.");
            }
        }
    }
}
