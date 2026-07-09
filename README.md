# 🎨 PixelSync - Sistema Distribuído de Pixel Art (REST API)

Este projeto é uma implementação de um sistema distribuído inspirada no *r/place*, onde múltiplos usuários podem pintar pixels em um quadro compartilhado em tempo real.

O projeto foi desenvolvido como parte da disciplina de **Sistemas Distribuídos** (Trabalho 3), utilizando uma arquitetura **REST API** para comunicação entre o servidor e clientes heterogêneos.

## 🚀 Tecnologias Utilizadas

* **Servidor:** Python (FastAPI + Uvicorn)
* **Cliente 1:** Web (HTML5, JavaScript Puro, CSS)
* **Cliente 2:** Desktop (C# .NET Windows Forms)
* **Comunicação:** HTTP/REST (JSON)

---

## 📋 Pré-requisitos

Para rodar este projeto, você precisará ter instalado em sua máquina:

1.  **Python 3.8+** (Para o servidor)
2.  **.NET SDK 6.0+** (Para o cliente C#)
3.  Um navegador moderno (Chrome, Firefox, Edge)

---

## 🛠️ Instalação e Execução

Siga a ordem abaixo para iniciar o sistema corretamente.

### 1. Iniciando o Servidor (Python)

O servidor é o coração do sistema. Ele deve estar rodando antes de abrir qualquer cliente.

1.  Abra o terminal na pasta do servidor.
2.  Instale as dependências necessárias:
    ```bash
    pip install fastapi uvicorn
    ```
3.  Execute o servidor:
    ```bash
    # Se o arquivo se chamar servidor_api.py
    uvicorn servidor_api:app --reload --host 0.0.0.0 --port 8000
    ```
    *Se o seu arquivo tiver outro nome, substitua `servidor_api` pelo nome do seu arquivo.*

O servidor estará rodando em `http://localhost:8000`.

---

### 2. Rodando o Cliente Web (JavaScript)

Este cliente roda diretamente no navegador.

1.  Navegue até a pasta onde está o arquivo `index.html`.
2.  Basta **abrir o arquivo `index.html`** com o seu navegador de preferência (duplo clique).
3.  Insira seu nome de usuário, escolha uma cor e comece a pintar.

*Nota: O servidor possui CORS habilitado, permitindo que o arquivo local se comunique com a API.*

---

### 3. Rodando o Cliente Desktop (C#)

Este cliente foi desenvolvido em .NET e roda nativamente no Windows/Linux/Mac (desde que tenha o runtime).

1.  Abra o terminal na pasta onde está o arquivo `WPlaceClient.cs` (ou a pasta do projeto .NET).
2.  Execute o projeto:
    ```bash
    dotnet run
    ```
3.  Uma janela do Windows Forms abrirá.

---

## 🎮 Como Usar

As funcionalidades são idênticas em ambos os clientes:

* **Pintar:** Clique com o botão esquerdo do mouse em qualquer quadrado do grid.
* **Arrastar:** Segure o clique e arraste o mouse para pintar múltiplos pixels continuamente.
* **Trocar Cor:** Clique no botão "Escolher Cor" (ou no input de cor na Web) para alterar a tinta.
* **Admin:** O botão "Limpar Tudo" envia uma requisição `DELETE` para o servidor, apagando todo o quadro para todos os usuários conectados.

## 👥 Autores

* **Francisco Leudes Bezerra Neto**
* **Erica de Castro Silveira**
