using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Signalr.desktop
{
    public partial class Form1 : Form
    {
        private HubConnection _hubConnection;
        private List<string> itemList = new List<string>();

        public Form1()
        {
            InitializeComponent();

            // Replace "http://localhost:5000/chatHub" with the URL of your SignalR hub endpoint
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7051/chatHub?name=desktop")
                .Build();

            // Receive messages from the server and update the output field
            _hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                Invoke(new Action(() =>
                {
                    txtOutput.Text = message;
                }));
            });


            _hubConnection.On<Dictionary<string, string>>("ReceiveClients", (clients) =>
            {
                Invoke(new Action(() =>
                {
                    itemList.Clear();
                    
                    foreach (KeyValuePair<string, string> item in clients)
                    {
                        string itemText = item.Value;
                        itemList.Add(itemText);
                    }
                
                    lstItems.Items.Clear();
                    lstItems.Items.AddRange(itemList.ToArray());
                }));
            });

            try
            {
                _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to SignalR hub: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtInput.Text))
            {
                try
                {
                    await _hubConnection.SendAsync("SendMessage", txtInput.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending message: {ex.Message}", "Send Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void lstItems_Select_Client(object sender, EventArgs e)
        {
            if (lstItems.SelectedItem != null)
            {
                string selectedItemText = lstItems.SelectedItem.ToString();
                this.txtClient.Text = selectedItemText;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClient.Text))
            {
                MessageBox.Show($"Please select a client", "Select a client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                try
                {
                    await _hubConnection.SendAsync("SendMessageToClient", txtClient.Text, txtInput.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending message: {ex.Message}", "Send Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
