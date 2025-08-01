using NewEmployeeData; //Employee data models (database)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NewEmployeeService.Controllers
{
    public class NewEmployeeController : ApiController
    {
        //GET method api/newemployee
        public IEnumerable<NewEmployee> Get()
        {
            // Open connection to database nd convert to a list
            using (NewEmployeeDBEntities entities = new NewEmployeeDBEntities())
            {
                return entities.NewEmployees.ToList();// Converts the employee table to a list and returns it
            }
        }

        // GET method with ID api/newemployee/{id}
        public HttpResponseMessage Get(int id)
        {
            using (NewEmployeeDBEntities entities = new NewEmployeeDBEntities())
            {
                var entity = entities.NewEmployees.FirstOrDefault(e => e.ID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);  //Send back a "200 OK" response with the employee data
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Employee with Id " + id.ToString() + " not found");
                }
            }
        }

        //POST method - when someone wants to create a new employee api/newemployee
        public HttpResponseMessage Post([FromBody] NewEmployee employee)
        {
            try
            {
                using (NewEmployeeDBEntities entities = new NewEmployeeDBEntities())
                {
                    entities.NewEmployees.Add(employee);// Adds employee to entity set
                    entities.SaveChanges(); //Automatically save the employee object

                    var message = Request.CreateResponse(HttpStatusCode.Created, employee); //201 status code
                    message.Headers.Location = new Uri(Request.RequestUri +
                        employee.ID.ToString()); // Adds URI location of the new employee

                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //DELETE method api/newemployee/{id}
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (NewEmployeeDBEntities entities = new NewEmployeeDBEntities())
                {
                    // Look for the employee we want to delete
                    var entity = entities.NewEmployees.FirstOrDefault(e => e.ID == id); //check if ID exist
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with Id = " + id.ToString() + " not found to delete"); // Return 404 Not Found error if NOT found
                    }
                    else
                    {
                        entities.NewEmployees.Remove(entity); // Remove employee
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex); // 400 Bad Request
            }
        }

        //POST method - when someone wants to update an existing employee api/newemployee/{id}
        public HttpResponseMessage Put(int id, [FromBody] NewEmployee employee)
        {
            try
            {
                using (NewEmployeeDBEntities entities = new NewEmployeeDBEntities())
                {
                    var entity = entities.NewEmployees.FirstOrDefault(e => e.ID == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with Id " + id.ToString() + " not found to update");
                    }
                    else
                    {   // Update employee details
                        entity.FirstName = employee.FirstName;
                        entity.LastName = employee.LastName;
                        entity.Age = employee.Age;
                        entity.Role = employee.Role;

                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
