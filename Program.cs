namespace Python_Scripts;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;
using System.Threading.Tasks;

// Classe simples para representar os dados (equivalente ao seu dataclass Python)
public class PixelData {
    public int x { get; set; }
    public int y { get; set; }
    public int r { get; set; }
    public int g { get; set; }
    public int b { get; set; }
    public string owner { get; set; }
}

public class WPlaceClient : Form {
    // Configurações
    private const string API_URL = "http://localhost:8000";
    private const int PIXEL_SIZE = 20;
    private const int ROWS = 30;
    private const int COLS = 30;

    // Estado
    private Dictionary<string, PixelData> pixels = new();
    private Color currentColor = Color.Black;
    private string username = "CSharpUser";
    private Point lastPainted = new Point(-1, -1);

    // Componentes
    private readonly HttpClient client = new HttpClient();
    private PictureBox canvas;
    private Panel colorPreview;

    public WPlaceClient() {
        this.Text = "WPlace - Cliente C#";
        this.Size = new Size(COLS * PIXEL_SIZE + 40, ROWS * PIXEL_SIZE + 120);
        this.DoubleBuffered = true; // Evita piscar a tela

        SetupUI();

        // Loop de atualização (Timer)
        var timer = new Timer { Interval = 1000 };
        timer.Tick += async (s, e) => await FetchBoard();
        timer.Start();
    }

    private void SetupUI() {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };

        // Botão Cor
        var btnColor = new Button { Text = "Cor" };
        colorPreview = new Panel { BackColor = currentColor, Size = new Size(20, 20), BorderStyle = BorderStyle.FixedSingle };
        btnColor.Click += (s, e) => {
            using var cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK) {
                currentColor = cd.Color;
                colorPreview.BackColor = currentColor;
            }
        };

        // Input Usuário
        var txtUser = new TextBox { Text = username, Width = 80 };
        txtUser.TextChanged += (s, e) => username = txtUser.Text;

        // Botão Limpar
        var btnClear = new Button { Text = "Limpar (Admin)", ForeColor = Color.Red, AutoSize = true };
        btnClear.Click += async (s, e) => {
            if (MessageBox.Show("Limpar tudo?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                await client.DeleteAsync($"{API_URL}/clear");
        };

        panel.Controls.AddRange(new Control[] { btnColor, colorPreview, new Label { Text = "User:", AutoSize = true, Padding = new Padding(0,6,0,0) }, txtUser, btnClear });

        // Canvas (Área de desenho)
        canvas = new PictureBox { 
            Size = new Size(COLS * PIXEL_SIZE + 1, ROWS * PIXEL_SIZE + 1), 
            BackColor = Color.White,
            Cursor = Cursors.Cross,
            Location = new Point(10, 50)
        };
        
        // Eventos de Pintura e Mouse
        canvas.Paint += Canvas_Paint;
        canvas.MouseDown += (s, e) => { if(e.Button == MouseButtons.Left) PaintPixel(e.X, e.Y); };
        canvas.MouseMove += (s, e) => { if(e.Button == MouseButtons.Left) PaintPixel(e.X, e.Y); };
        canvas.MouseUp += (s, e) => lastPainted = new Point(-1, -1);

        this.Controls.Add(panel);
        this.Controls.Add(canvas);
    }

    // --- LÓGICA DE DESENHO ---
    private void Canvas_Paint(object sender, PaintEventArgs e) {
        var g = e.Graphics;

        // 1. Desenha Pixels
        foreach (var p in pixels.Values) {
            using var brush = new SolidBrush(Color.FromArgb(p.r, p.g, p.b));
            g.FillRectangle(brush, p.x * PIXEL_SIZE + 1, p.y * PIXEL_SIZE + 1, PIXEL_SIZE - 2, PIXEL_SIZE - 2);
        }

        // 2. Desenha Grid
        using var pen = new Pen(Color.LightGray);
        for (int i = 0; i <= COLS; i++) g.DrawLine(pen, i * PIXEL_SIZE, 0, i * PIXEL_SIZE, ROWS * PIXEL_SIZE);
        for (int i = 0; i <= ROWS; i++) g.DrawLine(pen, 0, i * PIXEL_SIZE, COLS * PIXEL_SIZE, i * PIXEL_SIZE);
    }

    // --- LÓGICA DE REDE ---
    private async void PaintPixel(int mouseX, int mouseY) {
        int x = mouseX / PIXEL_SIZE;
        int y = mouseY / PIXEL_SIZE;

        if (x < 0 || x >= COLS || y < 0 || y >= ROWS) return;
        if (x == lastPainted.X && y == lastPainted.Y) return; // Evita flood

        lastPainted = new Point(x, y);

        // Feedback visual imediato
        var tempPixel = new PixelData { x=x, y=y, r=currentColor.R, g=currentColor.G, b=currentColor.B };
        pixels[$"{x},{y}"] = tempPixel; 
        canvas.Invalidate(); // Força redesenho local

        try {
            tempPixel.owner = username; // Adiciona owner para envio
            await client.PostAsJsonAsync($"{API_URL}/paint", tempPixel);
        } catch { /* Ignora erros de rede no cliente para não travar UI */ }
    }

    private async Task FetchBoard() {
        try {
            // O "Black Magic" do C#: Deserializa JSON direto para Dicionário de Objetos
            var result = await client.GetFromJsonAsync<Dictionary<string, PixelData>>($"{API_URL}/board");
            if (result != null) {
                pixels = result;
                canvas.Invalidate();
            }
        } catch { /* Servidor offline? */ }
    }

    [STAThread]
    static void Main() {
        Application.EnableVisualStyles();
        Application.Run(new WPlaceClient());
    }
}