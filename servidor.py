from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import Dict, List
from dataclasses import asdict

# Reutilizando sua lógica de negócio (simplificada para o exemplo)
class PixelData(BaseModel):
    x: int
    y: int
    r: int
    g: int
    b: int
    owner: str

class Board:
    def __init__(self, width=30, height=30):
        self.width = width
        self.height = height
        self.pixels = {} 

    def paint(self, p: PixelData):
        if 0 <= p.x < self.width and 0 <= p.y < self.height:
            key = f"{p.x},{p.y}"
            # Armazena como dicionário para facilitar o JSON
            self.pixels[key] = p.dict()
            return True
        return False

    def get_state(self):
        return self.pixels

    def clear(self):
        self.pixels = {}
        return True

# --- Configuração da API ---
app = FastAPI()
board = Board()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Permite qualquer origem (para desenvolvimento)
    allow_credentials=True,
    allow_methods=["*"],  # Permite GET, POST, DELETE, etc.
    allow_headers=["*"],
)

# Rota para obter o estado do quadro (GET)
@app.get("/board")
def get_board():
    return board.get_state()

# Rota para pintar um pixel (POST)
@app.post("/paint")
def paint_pixel(pixel: PixelData):
    success = board.paint(pixel)
    if not success:
        raise HTTPException(status_code=400, detail="Coordenadas inválidas")
    return {"status": "painted", "pixel": pixel}

# Rota para limpar o quadro (DELETE)
@app.delete("/clear")
def clear_board():
    board.clear()
    return {"status": "board cleared"}

# Para rodar: uvicorn servidor_api:app --reload --host 0.0.0.0 --port 8000