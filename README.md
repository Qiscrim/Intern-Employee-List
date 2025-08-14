# Admin Dashboard - User Management System

A comprehensive web-based user management dashboard for administrators to manage user accounts, roles, and permissions within the Unique Concept system.

## 🚀 Features

### User Management
- **View All Users**: Display users in a sortable, searchable table format
- **User Statistics**: Real-time count of total users, administrators, and regular users
- **User Search**: Search users by username or ID
- **Role Filtering**: Filter users by role (All, Administrators, Regular Users)

### User Operations
- **Add New Users**: Create new user accounts with username, password, and role assignment
- **Password Reset**: Administrators can reset user passwords
- **Role Management**: Promote users to administrators or demote to regular users
- **User Deletion**: Remove user accounts (with appropriate permissions)

### Security & Permissions
- **Multi-tier Admin System**: 
  - Root Administrators (highest level)
  - Regular Administrators (limited permissions)
  - Regular Users
- **Protected Operations**: Root admin accounts are protected from modification/deletion
- **Self-Protection**: Users cannot modify their own roles or delete themselves
- **Permission-based Actions**: Different actions available based on user privileges

## 🛠️ Technology Stack

- **Frontend**: HTML5, CSS3, JavaScript (jQuery)
- **Styling**: Bootstrap 5 + Custom CSS
- **Backend API**: ASP.NET Web API (assumed)
- **Authentication**: JWT Token-based authentication
- **Storage**: localStorage for session management

## 📁 Project Structure

```
project-root/
├── admin-dashboard.html          # Main dashboard page
├── admin-dashboardstyles.css     # Custom styling
├── Content/
│   └── bootstrap.min.css         # Bootstrap framework
├── Scripts/
│   └── bootstrap.min.js          # Bootstrap JavaScript
└── README.md                     # This file
```

## 🚦 Getting Started

### Prerequisites
- Web server (IIS, Apache, or any HTTP server)
- Backend API running on `http://localhost:57696/api`
- Modern web browser with JavaScript enabled

### Installation

1. **Clone/Download** the project files to your web server directory

2. **Ensure Dependencies**:
   - Bootstrap CSS and JS files are properly linked
   - jQuery 3.6.0 is loaded from CDN
   - Font Awesome icons (for UI elements)

3. **Configure API Endpoint**:
   ```javascript
   const API_BASE_URL = 'http://localhost:57696/api';
   ```
   Update this URL to match your backend API location.

4. **Access the Dashboard**:
   - Navigate to `admin-dashboard.html` in your web browser
   - Ensure you're logged in with administrator privileges

### Backend API Requirements

The dashboard expects the following API endpoints:

```
GET    /api/users              - Retrieve all users
POST   /api/users              - Create new user
PUT    /api/users/{id}/role     - Update user role
PUT    /api/users/{id}/password - Reset user password
DELETE /api/users/{id}         - Delete user
```

## 🔐 Authentication & Authorization

### Authentication Flow
1. User must be logged in with valid JWT token stored in localStorage
2. Token is sent with all API requests via Authorization header
3. Session validation occurs on page load

### User Roles & Permissions

| Action | Regular User | Admin | Root Admin |
|--------|--------------|-------|------------|
| View Users | ❌ | ✅ | ✅ |
| Add Users | ❌ | ✅ | ✅ |
| Reset Regular User Password | ❌ | ✅ | ✅ |
| Reset Admin Password | ❌ | ❌ | ✅ |
| Change Regular User Role | ❌ | ✅ | ✅ |
| Change Admin Role | ❌ | ❌ | ✅ |
| Delete Regular User | ❌ | ✅ | ✅ |
| Delete Admin | ❌ | ❌ | ✅ |
| Modify Root Admin | ❌ | ❌ | ❌ |

### Protected Operations
- **Root Admin Protection**: Root administrator accounts cannot be modified or deleted by anyone
- **Self-Protection**: Users cannot modify their own roles or delete their own accounts
- **Admin Hierarchy**: Regular administrators cannot modify other administrator accounts

## 🎨 User Interface

### Dashboard Components
- **Header**: Logo, administrator badge, welcome message
- **Statistics Cards**: Visual display of user counts
- **Search & Filter Controls**: Real-time user filtering
- **User Table**: Comprehensive user information display
- **Action Buttons**: Context-aware operation buttons
- **Modal Dialogs**: Add user and password reset forms

### Visual Indicators
- 👑 **Root Admin Badge**: Identifies root administrators
- 🛡️ **Admin Badge**: Identifies regular administrators
- 👤 **User Badge**: Identifies regular users
- 🔒 **Protected Indicators**: Shows protected accounts
- ⚡ **Recent Login**: Highlights recent user activity

## 🔧 Configuration

### LocalStorage Data Structure
```javascript
// Current user data
localStorage.setItem('currentUser', JSON.stringify({
    ID: 1,
    Username: "admin",
    IsAdmin: true,
    IsRootAdmin: true
}));

// Authentication token
localStorage.setItem('authToken', 'your-jwt-token-here');
```

### API Response Format
```javascript
// User object structure
{
    ID: 1,
    Username: "username",
    IsAdmin: false,
    IsRootAdmin: false,
    CreatedDate: "2023-01-01T00:00:00Z",
    LastLoginDate: "2023-12-01T10:30:00Z"
}
```

## 🚨 Error Handling

### Common Error Scenarios
- **401 Unauthorized**: Session expired, redirect to login
- **403 Forbidden**: Insufficient permissions for operation
- **404 Not Found**: User or resource not found
- **400 Bad Request**: Invalid data or duplicate username

### Client-Side Validation
- Username minimum length (3 characters)
- Password minimum length (6 characters)
- Role selection validation
- Permission checks before API calls

## 📱 Browser Compatibility

- **Chrome**: 60+ ✅
- **Firefox**: 55+ ✅
- **Safari**: 12+ ✅
- **Edge**: 79+ ✅
- **Internet Explorer**: Not supported ❌

## 🔒 Security Considerations

1. **JWT Token Security**: Tokens should have appropriate expiration times
2. **HTTPS**: Use HTTPS in production environments
3. **Input Validation**: Both client and server-side validation implemented
4. **Permission Verification**: Server-side permission checks required
5. **Protected Operations**: Critical operations require confirmation dialogs

## 🐛 Troubleshooting

### Common Issues

**Dashboard not loading users**
- Check if API is running on correct port
- Verify authentication token in localStorage
- Check browser console for JavaScript errors

**Permission denied errors**
- Verify user has administrator privileges
- Check if trying to modify protected accounts
- Ensure proper role hierarchy

### Design
-Registration
<img width="941" height="448" alt="image" src="https://github.com/user-attachments/assets/57944b0c-42fa-4034-9cdc-6ca03b3737bc" />
-Signin
<img width="956" height="449" alt="image" src="https://github.com/user-attachments/assets/1b4c57b5-6282-40f9-a6b1-e85e54b5c908" />
-Employee List (pagination, sidebar included)
<img width="950" height="446" alt="image" src="https://github.com/user-attachments/assets/b7ffad56-be48-4449-9a5c-4d4c9cf8b2a2" />
<img width="949" height="446" alt="image" src="https://github.com/user-attachments/assets/968fa739-0a32-4e99-b736-c9305434ed03" />
-Add Employee Page
<img width="944" height="446" alt="image" src="https://github.com/user-attachments/assets/5b5555da-1d59-4d5e-aa95-135272e1f4a9" />
-Admin Dashboard
<img width="941" height="444" alt="image" src="https://github.com/user-attachments/assets/7a28a025-3af7-44ba-a9ab-411b13773e24" />
<img width="938" height="446" alt="image" src="https://github.com/user-attachments/assets/8d11015d-f5d7-4b88-82c3-75c1ce10b887" />


