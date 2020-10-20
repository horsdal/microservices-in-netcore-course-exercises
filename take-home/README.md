# Take home exercise: Build the Loyalty Program microservices

## Goal

The goal of the exercise is to implement the first version of loyalty program microservice that we have seen as an example throughout day one. The focus is on first implementing the collaboration between the loyalty program and other microservices and second on the business logic of notifying loyalty program members of special offers.

This exercise is a "fill in the blanks" exercise: There is already a skeleton of the loyalty program and some very simple versions of the other microservices. The goal is implement the missing bits in the loyalty program microservice.

## Getting started

To run the environment needed for this exercise go to the `take-home` folder and run `docker-compose up -d`. This should start 3 containers which you can inspect with the `docker ps` command.

There are four microservices plus rabbitmq at play in this exercise:

 * RabbitMq was started by the docker-compose command and the management interface is available at http://localhost:15672 - the login is guest/guest
 * The special offers microservices which offers one POST endpoint for creating new special offers. Whenever that endpoint is called it will publish an event of the type `Contracts.SpecialOffers.SpecialOfferCreated` to rabbit. The special offers microservice was also start by docker-compose and offer an OpenApi UI for calling the endpoint at http://localhost:5000
 * The notifications microservice which offers two endpoints: One POST for sending notification and a GET for seeing all the commands to send notification that have been made to the POST endpoint. The GET endpoint is for debug purposes only. Notifications microservice is the thirds container started by docker-compose, also has an OpenApi UI and is available at http://localhost:5001
 * A mock of the API gateway that is supposed to sit in front of the loyalty program. This is a small console app that let's you send commands and queries to the loyalty program to registers user, update users and see the users. The api gateway mock will try to call endpoints in the loyalty program that aren't implemented yet :). You can run the api gateway by going to `take-home/ApiGatewayMock` and running `dotnet run`
 * Lastly there is the loyalty program which is in `take-home/LoyaltyProgram`. It is only a skeleton implementation at this point. It is not part of the docker-compose but can be run with `dotnet run` and will listen on localhost:6000.

You may also notice that there is a `take-home/Contracts` folder this contains types that are send over RabbitMQ (the ones in `take-home/Contracts/SpecialOffers`) or HTTP (the ones in `take-home/Contracts/Notifications`).

## Exercise 1: Implement the LoyaltyProgram API

In the `UsersController` implement the following endpoint:

 * `GET /users/{userid}` which returns the information about the user with id=userid. You can get the user from the `UserDb`.
 * `POST /users` which allows for registering a new user by sending user information as json in the body. You can the type `LoyaltyProgramUser` when deserializing the request body.
 * `PUT /users/{userid}` update the user with id=userid. Again the `LoyaltyProgramUser` type can be used.

The api gateway mock uses these three endpoints. The exercise is done when the API gateway can successfully call all three.

## Exercise 2: Subscribe to Special offers events

In the loyalty program there is a hosted background service called `SpecialOfferConsumer`. This gets started when the microservice starts and gets stopped when the microservice is stopped. Implement the `StartAsync` method so it subscribes to the `Contract.SpecialOffers.SpecialOfferCreated` events published by the special offers microservice. Use the `_logger` to write something to the console (`Console.WriteLine` wont work) whenever a special offer is created.

The exercise is done when you can got to http://localhost:5000 create a special offer and see the log output in the console of the loyalty program.


## Exercise 3: Send notifications to interested users

User have interests which can be setup through the API gateway mock. Special offers are also tagged with interests. Whenever a special offer is created match the special offer to the loyalty program members found in `UserDb` and send a notification to the user by calling the POST endpoint in the notifications services.

The exercise is done when you can register a user through the API gateway, attach some interests to the user, then create a special offer at http://localhost:5000 that matches the interests of the user and finally check that the notification microservice was asked to send a notification by inspecting the notifications GET endpoint at http://localhost:5001


:champagne: CELEBRATE :dancers:

## Trouble-shooting

### Docker-compose 

The output from `docker ps` should be similar to:

```
CONTAINER ID        IMAGE                 COMMAND                  CREATED             STATUS              PORTS                                                                                                         NAMES
004aa9385e09        notifications         "dotnet Notification…"   10 minutes ago      Up 10 minutes       443/tcp, 0.0.0.0:5001->80/tcp                                                                                 take-home_notifications_1
1fd30210fc14        rabbitmq:management   "docker-entrypoint.s…"   13 minutes ago      Up 13 minutes       4369/tcp, 5671/tcp, 0.0.0.0:5672->5672/tcp, 15671/tcp, 15691-15692/tcp, 25672/tcp, 0.0.0.0:15672->15672/tcp   take-home_rabbitmq_1
19802a509fc0        specialoffers         "dotnet SpecialOffer…"   19 minutes ago      Up 19 minutes       443/tcp, 0.0.0.0:5000->80/tcp                                                                                 take-home_specialoffers_1
```

if you change anything in one of the pre-built microservices make sure to rebuilt the docker images by running `docker-compose up -d --build`.

### RabbitMQ

The management UI is very useful. Check it out at http://localhost:15672. When the special offers get called it should create an exchange called `Contracts.SpecialOffers.SpecialOfferCreated, Contracts`. When you subscription is set up in loyalty program you can see it under bindings at http://localhost:15672/#/exchanges/%2F/Contracts.SpecialOffers.SpecialOfferCreated%2C%20Contracts

In certain failure scenarios EasyNetQ will create an error exchange and error queue. That can also help you debug.

Under each queue in the UI there is a `Get messages` part that allows you to see message currently on the queue. This is useful for seeing what's on the error queue or for seeing what is on your queue. Notice that when you subscription works it will probably be fast enough that the queue looks empty all the time.