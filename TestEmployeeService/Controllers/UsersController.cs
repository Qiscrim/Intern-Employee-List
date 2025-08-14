using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using System.Text;
using TestEmployeeDataAccess;

namespace TestEmployeeService.Controllers
{
    public class UsersController : ApiController
    {   

        // POST api/users/register
        [HttpPost]
        [Route("api/users/register")]
        public HttpResponseMessage Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username and password are required.");
                }

                if (request.Username.Length < 3)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username must be at least 3 characters long.");
                }

                if (request.Password.Length < 6)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Password must be at least 6 characters long.");
                }

                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    // Check if username already exists
                    var existingUser = entities.Users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower());
                    if (existingUser != null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username already exists.");
                    }

                    // Create new user
                    var user = new User
                    {
                        Username = request.Username,
                        PasswordHash = HashPassword(request.Password),
                        IsAdmin = false, // Default to non-admin
                        IsRootAdmin= false,// Default to non-rootadmin
                        CreatedDate = DateTime.Now,
                        LastLoginDate = null
                    };

                    entities.Users.Add(user);
                    entities.SaveChanges();

                    // Return success response (don't include sensitive data)
                    var response = new
                    {
                        Message = "User registered successfully",
                        Username = user.Username,
                        IsAdmin = user.IsAdmin
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Registration failed: " + ex.Message);
            }
        }

        // POST api/users/login
        [HttpPost]
        [Route("api/users/login")]
        public HttpResponseMessage Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username and password are required.");
                }

                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    // Find user by username
                    var user = entities.Users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower());
                    if (user == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid username or password.");
                    }

                    // Verify password
                    if (!VerifyPassword(request.Password, user.PasswordHash))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid username or password.");
                    }

                    // Update last login date
                    user.LastLoginDate = DateTime.Now;
                    entities.SaveChanges();

                    // Generate a simple token (in production, use JWT or proper token system)
                    var token = GenerateSimpleToken(user.ID, user.Username);

                    // Return success response
                    var response = new
                    {
                        Message = "Login successful",
                        Token = token,
                        User = new
                        {
                            ID = user.ID,
                            Username = user.Username,
                            IsAdmin = user.IsAdmin,
                            IsRootAdmin = user.IsRootAdmin,
                            LastLoginDate = user.LastLoginDate
                        }
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Login failed: " + ex.Message);
            }
        }

        // GET api/users (Admin only - get all users)
        [HttpGet]
        public HttpResponseMessage Get()
        {
            try
            {
                // TODO: Add proper authentication/authorization check here
                // For now, assuming this endpoint is accessed by admin

                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    var users = entities.Users.Select(u => new
                    {
                        ID = u.ID,
                        Username = u.Username,
                        IsAdmin = u.IsAdmin,
                        CreatedDate = u.CreatedDate,
                        LastLoginDate = u.LastLoginDate,
                        IsRootAdmin = u.IsRootAdmin
                        // Note: Never return password hashes for security
                    }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, users);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to retrieve users: " + ex.Message);
            }
        }

        // GET api/users/{id}
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    var user = entities.Users.FirstOrDefault(u => u.ID == id);
                    if (user == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found.");
                    }

                    var response = new
                    {
                        ID = user.ID,
                        Username = user.Username,
                        IsAdmin = user.IsAdmin,
                        CreatedDate = user.CreatedDate,
                        LastLoginDate = user.LastLoginDate,
                        IsRootAdmin = user.IsRootAdmin

                    };

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to retrieve user: " + ex.Message);
            }
        }

        // POST api/users (Admin only - create new user)
        [HttpPost]
        public HttpResponseMessage Post([FromBody] CreateUserRequest request)
        {
            try
            {
                // TODO: Add proper authentication/authorization check for admin

                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username and password are required.");
                }

                if (request.Username.Length < 3)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username must be at least 3 characters long.");
                }

                if (request.Password.Length < 6)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Password must be at least 6 characters long.");
                }

                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    // Check if username already exists
                    var existingUser = entities.Users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower());
                    if (existingUser != null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username already exists.");
                    }

                    // Create new user
                    var user = new User
                    {
                        Username = request.Username,
                        PasswordHash = HashPassword(request.Password),
                        IsAdmin = request.IsAdmin,
                        //IsRootAdmin= request.IsRootAdmin,
                        CreatedDate = DateTime.Now,
                        LastLoginDate = null
                    };

                    entities.Users.Add(user);
                    entities.SaveChanges();

                    // Return the created user (without sensitive data)
                    var response = new
                    {
                        ID = user.ID,
                        Username = user.Username,
                        IsAdmin = user.IsAdmin,
                        CreatedDate = user.CreatedDate,
                        LastLoginDate = user.LastLoginDate,
                        Message = "User created successfully"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to create user: " + ex.Message);
            }
        }

        // PUT api/users/{id}/password (Admin only - update user password)
        [HttpPut]
        [Route("api/users/{id:int}/password")]
        public HttpResponseMessage UpdatePassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                // TODO: Add proper authentication/authorization check for admin

                if (request == null || string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "New password is required.");
                }

                if (request.NewPassword.Length < 6)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Password must be at least 6 characters long.");
                }

                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {   // Find the user to update
                    var user = entities.Users.FirstOrDefault(u => u.ID == id);
                    if (user == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found.");
                    }

                    // ✅ NEW: Check if same password
                    if (VerifyPassword(request.NewPassword, user.PasswordHash))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            "New password cannot be the same as the current password. Please choose a different password.");
                    }
                    // Hash the new password and save it
                    user.PasswordHash = HashPassword(request.NewPassword);
                    entities.SaveChanges();

                    var response = new
                    {
                        Message = $"Password updated successfully for user: {user.Username}",
                        UserID = user.ID,
                        Username = user.Username
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to update password: " + ex.Message);
            }
        }

        // PUT api/users/{id}/role (Admin only - update user role)
        [HttpPut]
        [Route("api/users/{id:int}/role")]
        public HttpResponseMessage UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                // TODO: Add proper authentication/authorization check for admin

                if (request == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Role information is required.");
                }

                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    var user = entities.Users.FirstOrDefault(u => u.ID == id);
                    if (user == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found.");
                    }

                    // ✅ NEW: Prevent changing RootAdmin role
                    if (user.IsRootAdmin == true)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                            "Cannot modify root administrator role. Root admin role is protected.");
                    }

                    // Update role
                    user.IsAdmin = request.IsAdmin; // Set new value
                    entities.SaveChanges();

                    var response = new
                    {
                        Message = $"Role updated successfully for user: {user.Username}",
                        UserID = user.ID,
                        Username = user.Username,
                        IsAdmin = user.IsAdmin
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to update role: " + ex.Message);
            }
        }

        // DELETE api/users/{id} (Admin only - delete user)
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                // TODO: Add proper authentication/authorization check for admin

                using (TestEmployeeDBEntities entities = new TestEmployeeDBEntities())
                {
                    var user = entities.Users.FirstOrDefault(u => u.ID == id);
                    if (user == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found.");
                    }

                    // ✅ NEW: Prevent deleting RootAdmin
                    if (user.IsRootAdmin == true)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                            "Cannot delete root administrator account. Root admin is protected.");
                    }

                    // Prevent deleting the last admin user
                    if (user.IsAdmin)
                    {
                        var adminCount = entities.Users.Count(u => u.IsAdmin);
                        if (adminCount <= 1)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                                "Cannot delete the last administrator user. At least one admin must remain.");
                        }
                    }

                    string deletedUsername = user.Username;
                    entities.Users.Remove(user);
                    entities.SaveChanges();

                    var response = new
                    {
                        Message = $"User '{deletedUsername}' deleted successfully",
                        DeletedUserID = id,
                        DeletedUsername = deletedUsername
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to delete user: " + ex.Message);
            }
        }
        #region Helper Methods

        // Hash password using SHA256 (in production, use bcrypt or similar)
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password + "YourSaltHere"));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Verify password against hash
        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }

        // Generate a simple token (in production, use JWT)
        private string GenerateSimpleToken(int userId, string username)
        {
            var tokenData = $"{userId}|{username}|{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        #endregion
    }

    #region Request/Response Models

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UpdatePasswordRequest
    {
        public string NewPassword { get; set; }
    }

    public class UpdateRoleRequest
    {
        public bool IsAdmin { get; set; }
    }

    #endregion
}