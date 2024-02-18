# 🎥 Cinema Booking System

This is a coding challenge built entirely from scratch based on the requirements
in the `/docs` folder. This is build using the .NET framework and with CQRS / Vertical
slice architecture.



## 🛑 Error Handling


**Assumptions**
* When requesting a specific single item via a route, if that resource doesn't exist, the application should respond with `NotFound`.
* When requesting the *children* of a resource, if the resource doesn't exist, the application will return `NotFound`.  If the resource exists, but there are no children, then the application will not error and return an empty set.