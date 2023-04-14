﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGameSimple
{
    public partial class FormJoinServer : Form
    {

        public IPEndPoint EndpointAddress { get; set; }
        public string Username { get; set; }
        public FormJoinServer()
        {
            InitializeComponent();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbIP.Text))
            {
                MessageBox.Show("Enter a username.");
                return;
            }
            if (tbIP.Text.Length >= 30)
            {
                MessageBox.Show("username must be less that 30 characters.");
                return;
            }

            // handle loopback address if user inputs "localhost"
            if (tbIP.Text.Trim().ToLower() == "localhost") tbIP.Text = "127.0.0.1";
            if (IPEndPoint.TryParse($"{tbIP.Text}:{tbPort.Text}", out IPEndPoint ipEndpoint))
            {
                Username = tbUsername.Text;
                EndpointAddress = ipEndpoint;
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid server address, check you Ip and port then try again.");
            }
        }
    }
}
