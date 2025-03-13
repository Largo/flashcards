# Flashcard Application with Spaced Repetition

A full-stack flashcard application built with C# Web API, Entity Framework Core, SQLite, and Vue.js. The application implements the SM-2 spaced repetition algorithm to help users learn and remember information more effectively.

## Features

- Create, read, update, and delete flashcards
- Organize flashcards into categories
- Study flashcards using the SM-2 spaced repetition algorithm
- Track progress and review history
- Responsive UI for desktop and mobile devices

## Tech Stack

### Backend
- .NET Web API
- Entity Framework Core
- SQLite Database
- C# 9.0+

### Frontend
- Vue.js 3
- Vue Router
- Pinia (State Management)
- Bootstrap 5
- Axios

## Project Structure

```
flashcards/
├── backend/                      # .NET Web API project
│   ├── FlashcardApi/            # Main API project
│   │   ├── Controllers/         # API controllers
│   │   ├── Models/              # Domain models
│   │   ├── Data/                # EF Core context and migrations
│   │   ├── Services/            # Business logic services
│   │   ├── Program.cs           # Application entry point
│   │   ├── appsettings.json     # Configuration
│   │   └── FlashcardApi.csproj  # Project file
│   └── FlashcardApi.sln         # Solution file
│
└── frontend/                     # Vue.js frontend
    ├── public/                   # Static assets
    ├── src/                      # Vue source code
    │   ├── assets/              # Frontend assets
    │   ├── components/          # Vue components
    │   ├── views/               # Vue views/pages
    │   ├── services/            # API services
    │   ├── App.vue              # Root component
    │   └── main.js              # Entry point
    ├── package.json             # NPM dependencies
    └── vite.config.js           # Vite configuration
```

## Spaced Repetition Algorithm

This application implements the SM-2 algorithm for spaced repetition, which works as follows:

1. After each review, the user rates their recall quality on a scale of 0-5:
   - 0: Complete blackout
   - 1: Incorrect response; the correct one remembered
   - 2: Incorrect response; where the correct one seemed easy to recall
   - 3: Correct response, but with difficulty
   - 4: Correct response, after some hesitation
   - 5: Perfect response

2. Based on this rating, the algorithm calculates:
   - The new "easiness factor" (EF)
   - The interval before the next review
   - Whether to reset the repetition count

3. Cards with lower ratings are shown more frequently, while well-known cards are shown less often.

## Setup and Running

### Backend

1. Navigate to the backend directory:
   ```
   cd backend
   ```

2. Run the API:
   ```
   cd FlashcardApi
   dotnet run
   ```

3. The API will be available at `https://localhost:5001/api`

### Frontend

1. Navigate to the frontend directory:
   ```
   cd frontend
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Run the development server:
   ```
   npm run dev
   ```

4. The frontend will be available at `http://localhost:5173`

## Database

The application uses SQLite for data storage. The database file is created in the backend directory when the application runs for the first time.

## License

MIT
