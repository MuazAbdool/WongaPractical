import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-gray-800 text-white px-6 py-4 flex gap-6 items-center">
      <Link to="/" className="text-white no-underline hover:text-primary transition-colors">
        Home
      </Link>
      {isAuthenticated ? (
        <>
          <Link to="/me" className="text-white no-underline hover:text-primary transition-colors">
            Profile
          </Link>
          <button 
            onClick={handleLogout} 
            className="ml-auto px-4 py-2 bg-primary hover:bg-primaryHover text-white rounded cursor-pointer transition-colors"
          >
            Logout
          </button>
        </>
      ) : (
        <>
          <Link to="/login" className="text-white no-underline hover:text-primary transition-colors">
            Login
          </Link>
          <Link to="/register" className="text-white no-underline hover:text-primary transition-colors">
            Register
          </Link>
        </>
      )}
    </nav>
  );
}


