
# ToDo System

A To-Do List application that allows users to create, read, update, and delete tasks, as well as retrieve weather information using an external API. Built using .NET 8, Entity Framework,SQLite,Serilog and a Weather API.

## **Table of Contents**
- [Overview](#overview)
- [Features](#features)
- [Technologies](#technologies)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [Usage](#usage)
  - [API Endpoints](#api-endpoints)
- [Testing](#testing)

  
## **Overview**

The ToDo System is a simple yet powerful to-do list application that allows users to manage their tasks and get real-time weather updates for their location. Tasks can be added, updated, deleted, and retrieved through the API, while the weather information is fetched using a third-party weather API.

## **Features**
- **Create, Update, Delete, and Retrieve Tasks**: Manage your tasks with various actions.
- **Weather Integration**: Get real-time weather information based on latitude and longitude.
- **In-Memory Database**: For testing purposes, the application uses an in-memory database to store tasks.

## **Technologies**
- **.NET  8.0**
- **Entity Framework Core 9.0**
- **SQLite** for persistent storage
- **Serilog** for structured logging
- **In-Memory Database for testing**
- **Moq for unit testing**
- **WeatherAPI for weather data**
- **Swagger for API documentation**
- **NUnit for unit testing**


## **Getting Started**

### **Prerequisites**
Before you begin, ensure you have the following installed:

- [.NET SDK 8.0 ](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://code.visualstudio.com/) (or any other IDE)

### **Installation**

1. **Install dependencies**:
    ```bash
    dotnet restore
    ```

2. **Set up the Weather API key**:
    - Sign up for a [WeatherAPI key](https://www.weatherapi.com/).
    - Add the API key in the environment variables.

3. **Install SQLite**:
    - SQLite is used to persist tasks in a lightweight local database. To install SQLite:
        - **Windows**: Download the [SQLite tools for Windows](https://www.sqlite.org/download.html) and install them.
        - **macOS**: SQLite is pre-installed on macOS, so you don't need to do anything.
        - **Linux**: You can install SQLite with the following command:
          ```bash
          sudo apt-get install sqlite3
          ```

4. **Install Serilog for Logging**:
    - **Serilog** is used for logging various events and errors in a structured format. To install the required packages for Serilog:
      ```bash
      dotnet add package Serilog
      dotnet add package Serilog.Sinks.Console
      dotnet add package Serilog.Sinks.File
      ```

### **Running the Application**

1. **Run the application**:
    ```bash
    dotnet run
    ```

2. Visit `https://localhost:44366/swagger` to use the application.

---

## **Usage**

### **API Endpoints**

Here are the available API endpoints for the application:

1. **GET** `https://localhost:44366/api/ToDo` - Retrieve all tasks
    - **Response**:
    ```json
    [{
    "id": 2,
    "title": "Memorize a poem",
    "completed": true,
    "userId": 13,
    "priority": 3,
    "latitude": null,
    "longitude": null,
    "dueDate": null,
    "categoryId": null,
    "category": null,
    "weather": null
  },
  {
    "id": 3,
    "title": "Pay Rent",
    "completed": false,
    "userId": 54,
    "priority": 1,
    "latitude": 51.5085,
    "longitude": -0.1257,
    "dueDate": "2025-04-06T06:46:39.761",
    "categoryId": 1,
    "category": {
      "id": 1,
      "title": "work",
      "parentCategoryId": 1
    },
    "weather": {
      "temperature": 18.3,
      "condition": "Sunny"
    }
  },
  ]
    ```

2. **GET** `/api/ToDo/{id}` - Retrieve a task by ID

3. **POST** `/api/ToDo` - Create a new task
    - **Request Body**:
    ```json
     {
        "title": "check final list",
  "completed": true,
  "userId": 0,
  "priority": 3,
  "latitude": 0,
  "longitude": 0,
  "dueDate": "2025-04-05T13:24:13.780Z",
  "categoryId": 0
  }
    ```

4. **PUT** `/api/ToDo/{id}` - Update an existing task
    - **Request Body**:
    ```json
    {
      
  "title": "pay rent",
  "completed": false,
  "userId": 23,
  "priority": 1,
  "latitude": 0,
  "longitude": 0,
  "dueDate": "2025-04-05T13:26:50.896Z",
  "categoryId": 1

    }
    ```

5. **DELETE** `/api/ToDo/{id}` - Delete a task by ID

6. **GET** `/api/ToDo/search?title=pay' - search by title, duedate, priority
    - **Response**:
    ```json
   {
      
  "title": "pay rent",
  "completed": false,
  "userId": 23,
  "priority": 1,
  "latitude": 0,
  "longitude": 0,
  "dueDate": "2025-04-05T13:26:50.896Z",
  "categoryId": 1

    }
    ```

## **Testing**

To run the unit tests for this application, follow these steps:

1. **Run the tests**:
    ```bash
    dotnet test
    ```

2. This will run all unit tests written for the application, including database operations, API calls, and more.

3. If you want to test specific functionalities (such as the ToDo Service or Weather API), mock dependencies like the `WeatherAPIService` or `TodoContext` in the unit tests.



