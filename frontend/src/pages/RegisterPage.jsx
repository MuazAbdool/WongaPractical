import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function RegisterPage() {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  const validate = () => {
    if (!firstName) return 'First name is required.';
    if (!lastName) return 'Last name is required.';
    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Valid email is required.';
    if (password.length < 6) return 'Password must be at least 6 characters.';
    return null;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    setError('');
    setLoading(true);
    try {
      await register(firstName, lastName, email, password);
      navigate('/me');
    } catch (err) {
      setError(err.response?.data?.error || 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto my-8 p-8 border border-gray-300 rounded-lg shadow-lg">
      <h2 className="text-2xl font-bold mb-6">Register</h2>
      {error && <p className="text-red-600 mb-4 font-semibold">{error}</p>}
      <form onSubmit={handleSubmit}>
        <div className="mb-4">
          <label className="block font-semibold">First Name</label>
          <input
            type="text"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            className="block w-full p-2 mt-1 border border-gray-400 rounded focus:outline-none focus:border-primary"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block font-semibold">Last Name</label>
          <input
            type="text"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            className="block w-full p-2 mt-1 border border-gray-400 rounded focus:outline-none focus:border-primary"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block font-semibold">Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="block w-full p-2 mt-1 border border-gray-400 rounded focus:outline-none focus:border-primary"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block font-semibold">Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="block w-full p-2 mt-1 border border-gray-400 rounded focus:outline-none focus:border-primary"
            required
          />
        </div>
        <button 
          type="submit" 
          disabled={loading} 
          className="w-full py-3 bg-primary text-white font-semibold rounded hover:bg-primaryHover disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          {loading ? 'Registering...' : 'Register'}
        </button>
      </form>
      <p className="mt-4 text-center">
        Already have an account? <Link to="/login" className="text-primary hover:text-primaryHover font-semibold">Login</Link>
      </p>
    </div>
  );
}
