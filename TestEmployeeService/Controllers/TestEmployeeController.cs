using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestEmployeeDataAccess;

namespace TestEmployeeService.Controllers
{
    public class TestEmployeeController : ApiController
    {
        //GET method api/testemployee
        public IEnumerable<TestEmployee> Get()
        {
            // Open connection to database nd convert to a list
            using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
            {
                return entities.TestEmployees.ToList();// Converts the employee table to a list and returns it
            }
        }

        // GET method with ID api/testemployee/{id}
        public HttpResponseMessage Get(int id)
        {
            using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
            {
                var entity = entities.TestEmployees.FirstOrDefault(e => e.ID == id);
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

        //POST method - when someone wants to create a new employee api/testemployee
        public HttpResponseMessage Post([FromBody] TestEmployee employee)
        {
            try
            {
                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    entities.TestEmployees.Add(employee);// Adds employee to entity set
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

        //[Authorize(Roles = "Admin")]
        //DELETE method api/testemployee/{id}
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    // Look for the employee we want to delete
                    var entity = entities.TestEmployees.FirstOrDefault(e => e.ID == id); //check if ID exist
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with Id = " + id.ToString() + " not found to delete"); // Return 404 Not Found error if NOT found
                    }
                    else
                    {
                        entities.TestEmployees.Remove(entity); // Remove employee
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

        //POST method - when someone wants to update an existing employee api/testemployee/{id}
        public HttpResponseMessage Put(int id, [FromBody] TestEmployee employee)
        {
            try
            {
                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    var entity = entities.TestEmployees.FirstOrDefault(e => e.ID == id);
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
