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
    public partial class Form1 : Form
    {
        Database database = new Database();
        SQLiteCommand command;
        DataTable dataTable;
        SQLiteDataAdapter adapter;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Login felület. Felhasználónév és jelszó megadása, szerepkör kiválasztása.
        //Megadott adatok ellenőrzése az adatbázisban.
        //Jegyvásárló felület vagy admin felület megnyitása.
        private void logInButton_Click(object sender, EventArgs e)
        {
            string selectedItemInComboBox = "";

            command = new SQLiteCommand("select * from User where username='" + usernameTextBox.Text + "' and password = '" + passwordTextBox.Text + "'", database.getConnection());
            adapter = new SQLiteDataAdapter(command);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            if(roleComboBox.SelectedIndex == 0 || roleComboBox.SelectedIndex == 1)
            {
                selectedItemInComboBox = roleComboBox.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Válassz szerepkört.");
            }


            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["role"].ToString() == selectedItemInComboBox)
                    {
                        MessageBox.Show("Beléptél " + dataTable.Rows[i]["role"] + " szerepkörrel.");
                        if (roleComboBox.SelectedIndex == 0)
                        {
                            TicketBuyForm ticketbuyform = new TicketBuyForm();
                            this.Hide();
                            ticketbuyform.Show();
                        }
                        else
                        {
                            AdminForm adminform = new AdminForm();
                            this.Hide();
                            adminform.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Rossz szerepkört választottál.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Hibás felhasználói név vagy jelszó.");
            }
        }

        //Alkalmazás bezárása.
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        //Alkalmazás bezárása.
        private void exitButton_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
    
