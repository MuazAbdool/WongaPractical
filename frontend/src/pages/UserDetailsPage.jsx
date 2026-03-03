import { useState, useEffect } from 'react';
import axiosInstance from '../api/axiosInstance';

export default function UserDetailsPage() {
  const [user, setUser] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    axiosInstance.get('/api/auth/me')
      .then((res) => setUser(res.data))
      .catch(() => setError('Failed to load user details.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p className="p-8 text-center">Loading...</p>;
  if (error) return <p className="p-8 text-center text-red-600 font-semibold">{error}</p>;

  return (
    <div className="max-w-lg mx-auto my-8 p-8 border border-gray-300 rounded-lg shadow-lg">
      <h2 className="text-2xl font-bold mb-6">Profile</h2>
      <p className="mb-2"><strong>First Name:</strong> {user.firstName}</p>
      <p className="mb-2"><strong>Last Name:</strong> {user.lastName}</p>
      <p className="mb-2"><strong>Email:</strong> {user.email}</p>
      <p className="mb-2"><strong>Member Since:</strong> {new Date(user.createdAt).toLocaleDateString()}</p>
    </div>
  );
}
