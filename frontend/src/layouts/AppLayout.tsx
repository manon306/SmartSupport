import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useState } from 'react';
// import { useSignalR } from '../context/SignalRContext';

export const AppLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  
  // To be implemented: const { notifications, unreadCount } = useSignalR();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const navItems = [
    { name: 'Dashboard', path: `/${user?.role.toLowerCase()}/dashboard`, roles: ['Employee', 'Agent', 'Admin'] },
    { name: 'My Tickets', path: '/employee/tickets', roles: ['Employee'] },
    { name: 'All Tickets', path: '/agent/tickets', roles: ['Agent'] },
    { name: 'Admin Tickets', path: '/admin/tickets', roles: ['Admin'] },
    { name: 'User Management', path: '/admin/users', roles: ['Admin'] },
  ];

  const filteredNavItems = navItems.filter(item => 
    user?.role && item.roles.includes(user.role)
  );

  return (
    <div className="min-h-screen flex flex-col md:flex-row bg-gray-100">
      {/* Mobile Sidebar Toggle */}
      <div className="md:hidden bg-white p-4 shadow flex justify-between items-center z-20">
        <span className="text-xl font-bold text-primary">SmartSupport</span>
        <button 
          onClick={() => setIsSidebarOpen(!isSidebarOpen)}
          className="text-gray-500 hover:text-gray-700"
        >
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>
      </div>

      {/* Sidebar */}
      <div className={`
        fixed inset-y-0 left-0 w-64 bg-white shadow-xl z-10 transform transition-transform duration-300 ease-in-out md:relative md:translate-x-0
        ${isSidebarOpen ? 'translate-x-0' : '-translate-x-full'}
      `}>
        <div className="h-full flex flex-col">
          <div className="p-6 hidden md:block">
            <span className="text-2xl font-bold text-primary tracking-tight">SmartSupport</span>
          </div>
          
          <div className="px-4 py-2 mt-4 md:mt-0 mb-6">
            <div className="p-3 bg-blue-50 rounded-lg border border-blue-100">
              <p className="text-sm text-gray-500 font-medium">Logged in as</p>
              <p className="font-semibold text-gray-800 truncate" title={user?.email}>{user?.fullName || user?.email}</p>
              <span className="inline-block mt-1 px-2 py-1 text-xs font-semibold bg-blue-100 text-blue-800 rounded-full">
                {user?.role}
              </span>
            </div>
          </div>

          <nav className="flex-1 px-4 space-y-1">
            {filteredNavItems.map((item) => {
              const isActive = location.pathname.startsWith(item.path);
              return (
                <Link
                  key={item.path}
                  to={item.path}
                  className={`block px-4 py-3 text-sm font-medium rounded-lg transition-colors duration-150 ${
                    isActive 
                      ? 'bg-primary text-white shadow-md' 
                      : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                  }`}
                  onClick={() => setIsSidebarOpen(false)}
                >
                  {item.name}
                </Link>
              );
            })}
          </nav>

          <div className="p-4 mt-auto border-t">
            <button
              onClick={handleLogout}
              className="flex w-full items-center justify-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-red-600 bg-red-50 hover:bg-red-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 transition-colors"
            >
              Log out
            </button>
          </div>
        </div>
      </div>

      {/* Main content */}
      <div className="flex-1 flex flex-col overflow-hidden relative">
        {/* Top Header */}
        <header className="bg-white shadow-sm z-0">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between h-16 items-center">
              <h2 className="text-xl font-semibold text-gray-800">
                {filteredNavItems.find(item => location.pathname.startsWith(item.path))?.name || 'Dashboard'}
              </h2>
              <div className="flex items-center space-x-4">
                {/* Notification Badge Placeholder */}
                <div className="relative p-2 text-gray-400 hover:text-gray-500 cursor-pointer">
                  <span className="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white transform translate-x-1/4 -translate-y-1/4 bg-red-600 rounded-full">
                    3
                  </span>
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                  </svg>
                </div>
              </div>
            </div>
          </div>
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-y-auto bg-gray-50 p-4 sm:p-6 lg:p-8">
          <Outlet />
        </main>
      </div>
      
      {/* Overlay for mobile sidebar */}
      {isSidebarOpen && (
        <div 
          className="fixed inset-0 bg-gray-600 bg-opacity-50 z-0 md:hidden"
          onClick={() => setIsSidebarOpen(false)}
        ></div>
      )}
    </div>
  );
};
