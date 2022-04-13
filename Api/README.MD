# The Last Hotel
The mission was to provide an API for booking the unique room available in the last hotel with a vacancy.

**Requirements
 - Three days as the maximum period .
 - Reservation can be scheduled starting at day after and no more than 30 days in advance.
 - A valid day starts at 00:00 and finishes at 23:59.
 - Actions: check room availability, place/change/cancel a reservation.
 - No authentication is required.

This API was designed:
- To support multiple GUI consumers.
- TDD and the SOLID principles was applied during the development process.
- Using Async/Await patterns thinking in a real-life scenario (high availability).
- Ready for containerization.

# Technologies
- DotNetCore 2.2.
- WebApi.
- NUnit (Testing).
- Autofac (Dependancy Injection).
- Swagger/Swashbuckle (auto-documented API).

# How to access The Swagger
Using Visual Studio, it will open the following route for swagger https://localhost:44356/swagger/index.html through the `IIS Express`.

# Routes and samples
There are six operations available using HTTP methods as following:
 - Get all reservations: GET https://localhost:44356/api/book/all
 - Check if the room is available for reservation: GET https://localhost:44356/api/book/check?start=2022-04-10&end=2022-04-13
 - Get a reservation by ID: GET https://localhost:44356/api/book?id=book22
 - Books a room: POST https://localhost:44356/api/book, message body:
```json
{
  "id": "book22",
  "startDate": "2022-04-13",
  "endDate": "2022-04-15",
  "numberOfBeds": 1
}
```
 - Change a reservation: PUT https://localhost:44356/api/book
```json
{
  "id": "book22",
  "startDate": "2022-04-13",
  "endDate": "2022-04-15",
  "numberOfBeds": 1
}
``` 
 - Cancel a reservation: DELETE https://localhost:44356/api/book?id=book22

# Problem approach
Firstly, to achieve a good understanding of the problem, small research was made to see how a reservation site works, like booking.com, looking at what are used as parameters and how the response was built.

Also, to help the development and the improvement process, the Model View Controller Architecture was applied, aiming to meet the requirements in the construction of a project that can be scalable and that possible changes in any of the layers are made without interference in the other layers. MVC is based on the separation of data (model), user interface (view), and business logic (controller).

Using this design approach the following segregation was applied:
 - **Model**: items related with the data manipulation, basically entities classes.
 - **View**: swagger and Postman (also considering the common GUI consumers).
 - **Controller**: the logic applied, the controller and the engine under the hood (Booking logic).

With this in mind was more accessible and more comfortable to provide a clean and robust code. The intent of separating each action as little as possible in each method was almost entirely achieved.

After these items are defined, the problem tackle begins.

The minimal data should have at least a unique ID, the start date, and the stay period. After confirming these items were working well, the number of beds was added to the booking data structure to ensure flexibility.

Considering the time and as the focus wasn't a consistency, no database integration was used. A singleton object to store the simulation data was created. It allows testing the behavior regarding the data manipulation and query.

The first method developed was the booking validation to confirm if the minimum requirements were fulfilled, like no empty booking id and a valid start/end date. To complete the requirements validation, the dates limitations were checked as the reservation start, advance, and room availability.

As the requirements were simple regarding the room availability, a small validation was adopted to confirm if there is any conflict dates reservation, verifying the request dates with is stored. One possible approach to improve this validation could be a register containing the room and its booked dates. For instance, It is also possible to include room types like standard, luxury, presidential, etc.

After all basic methods and verifications were completed, the **Operators** (Insert, Update, Cancel)  development was stated. Considering all validations are made before taking any consistent action, these methods were simpler to be completed. Insert creates a new booking object and stores it inside the list. The update method gets an existing object and applies the modifications. The cancel method removes a booking object from the list referred by an ID.

The last development step were the HTTP methods. The intent was to keep a single method for each HTTP item, easy to track and understand:
 - **GET** method was designed to bring a unique or all booking objects.
 - **POST** method registers the booking object by evaluating the unique ID.
 - **PUT** method updates an existing booking object referenced by its ID.
 - **DELETE** method cancel an existing booking object referenced by its ID removing it from the singleton data consistency.

With all under the hood parts completed, the development of the controller class that's operates the APIs started. The APIs routes and nouns are the essential part, they need to be consistent and easy to get what they are made for, and their use must be simple. Basically, everything starts at `/api/book`, only the method to get all reservations and the room availability check has a unique path (multiple GET methods), `api/book/all` and `api/book/check` respectively.

# Considerations
Follow some thoughts regarding design and features improvements:
 - NLog with multiple targets, a global exception handler
 - Authentication and authorization support, a good start would be eith OpenId/SSO.
 - Data consistency (database or repository)
 - Parameterization and configuration to allow custom limits and properties (multi-tenanted)
 - ordering ability (right now a booking request returns a booking response... 
 - room availability improvement, returning all available rooms
 - solve the race conditions, booking responses should expire 5 minutes unless the order were completed before achieving this timeout.
 - auto-assigned ID to prevent unwise manipulation.
