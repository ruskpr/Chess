﻿using Chess.Core.UDP;
using Chess.Core;
using Newtonsoft.Json;

namespace ChessGameSimple
{
    public partial class FormUDPClient : Form
    {
        string _username;
        private UdpUser _client;

        private Button[,] _buttons = new Button[8, 8];

        private Board _board;
        private Tile? _selectedTile = null;
        private Player _clientPlayer;
        private char _turn;

        public FormUDPClient(string name, string ip, int port)
        {
            InitializeComponent();

            _username = name;
            this.Text = "Chess - " + _username;

            _client = UdpUser.ConnectTo(_username, ip, port);
            _client.OnPacketRecieved += Client_OnPacketRecieved;
            _client.Listen();
        }

        #region private

        // send a packet to notify the server an updated game state is needed
        private void SendUpdateGameRequest()
        {
            Packet p = new(_username, $"{_username} requested a game update", PacketType.GameUpdateRequest);
            _client.Send(p);
        }

        // will be called when a packet is recieved from the server
        private void Client_OnPacketRecieved(Packet packet)
        {
            HandlePacket(packet);
        }

        private void HandlePacket(Packet packet)
        {
            // TODO: update board
            switch (packet.Type)
            {
                case PacketType.GameUpdateResponse:
                    UpdateGame(packet);
                    break;
                case PacketType.Disconnect:
                    throw new NotImplementedException();
                    break;
                case PacketType.Message:
                    this.Invoke(() => lstMessages.Items.Add($"{packet.SenderName}: {packet.Payload}"));

                    break;
                case PacketType.Move:
                    SendUpdateGameRequest();
                    throw new NotImplementedException();
                case PacketType.GameStart:
                    SendUpdateGameRequest();
                    break;
                case PacketType.GameEnd:
                    break;
                case PacketType.Error:
                    break;
                default:
                    break;
            }

        }

        private void UpdateGame(Packet packet)
        {
            // packet payload will be board as json
            // deserizalize packet payload
            var game = JsonConvert.DeserializeObject<Game>(packet.Payload, new JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            });

            _board = game.Board;
            
            if (_clientPlayer == null)
                _clientPlayer = game.Player1.Name == _username ? game.Player1 : game.Player2;

            _turn = game.Turn;

            this.Invoke(() =>
            {
                ChessUtils.CreateTiles(pnlBoard, _buttons, _board, pnlBoard.Width / 8, Color.Gainsboro, Color.Tan, OnTileClicked);
                ChessUtils.DrawSymbols(_buttons, _board);
            });
        }

        private void OnTileClicked(object? sender, EventArgs e)
        {
            Button btn = (Button)sender;
            BoardPoint boardPoint = (BoardPoint)btn.Tag;
            int row = boardPoint.row;
            int col = boardPoint.col;

            lstMessages.Items.Add($"{row}, {col}");

            // if no piece is selected, select the piece
            if (_selectedTile == null)
            {
                var piece = _board.GetPiece(row, col);

                if (piece != null)
                {
                    Tile selectedTile = _board.GetTile(row, col);

                    //if (selectedTile.Piece.Color != _clientPlayer.Symbol) return;

                    _selectedTile = selectedTile;
                    ChessUtils.ShowMoves(_buttons, _board, _selectedTile);
                    lstMessages.Items.Add($"Selected {_selectedTile.Piece.Symbol} at {_selectedTile.Row}, {_selectedTile.Column}");
                }
            }
            // unselect the piece if it is already selected
            else if (_selectedTile == _board.GetTile(row, col))
            {
                ChessUtils.HideMoves(_buttons);
                _selectedTile = null;
            }
            // if a piece is selected, try to move it
            else
            {
                Tile to = _board.GetTile(row, col);

                // if the piece is the same color as the selected piece, select the new piece
                if (to.Piece != null)
                {
                    if (_selectedTile.Piece.Color == to.Piece.Color)
                    {
                        _selectedTile = to;
                        ChessUtils.HideMoves(_buttons);
                        ChessUtils.ShowMoves(_buttons, _board, _selectedTile);
                        return;
                    }
                }

                bool moveSuccessful = _board.TryMakeMove(_selectedTile, to);

                if (moveSuccessful)
                {
                    ChessUtils.HideMoves(_buttons);
                    _selectedTile = null;
                    ChessUtils.DrawSymbols(_buttons, _board);
                    return;
                }
                else
                {
                    _selectedTile = null;
                    return;
                }
            }

        }

        #endregion

        #region form code

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (tbMessage.Text.Trim() != "")
                _client.Send(new Packet(_username, tbMessage.Text, PacketType.Message));

            tbMessage.Text = "";
        }

        private void FormClient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode > Keys.A && e.KeyCode < Keys.Z)
                tbMessage.Focus();
        }

        #endregion


    }
}