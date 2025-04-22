import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import { ChangeEvent } from 'react';

type User = {
  username: string;
  displayName: string;
  domain: string;
  isEnabled: boolean;
  isRdpEnabled: boolean;
};

const UserManagement = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [username, setUsername] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [domain, setDomain] = useState('');
  const [isEnabled, setIsEnabled] = useState(false);
  const [isRdpEnabled, setIsRdpEnabled] = useState(false);
  const router = useRouter();

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await fetch('/users');
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      const data: User[] = await response.json();
      setUsers(data);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  const addUser = async () => {
    try {
      const response = await fetch('/users', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username, displayName, domain, isEnabled, isRdpEnabled }),
      });
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      fetchUsers(); // Refresh the user list after adding
      setUsername('');
      setDisplayName('');
      setDomain('');
      setIsEnabled(false);
      setIsRdpEnabled(false);
    } catch (error) {
      console.error('Error adding user:', error);
    }
  };

  const enableUser = async (username: string) => {
    try {
      const response = await fetch(`/users/${username}/enable`, { method: 'PUT' });
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      fetchUsers();
    } catch (error) {
      console.error('Error enabling user:', error);
    }
  };

  const disableUser = async (username: string) => {
    try {
      const response = await fetch(`/users/${username}/disable`, { method: 'PUT' });
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      fetchUsers();
    } catch (error) {
      console.error('Error disabling user:', error);
    }
  };

  const resetPassword = async (username: string) => {
    try {
      const response = await fetch(`/users/${username}/reset-password`, { method: 'POST' });
      const data = await response.text();
      alert(data);
    } catch (error) {
      console.error('Error resetting password:', error);
    }
  };

  return (
    <div>
      <h1>RD User Management</h1>
      <button onClick={() => router.push('/')} className="px-4 py-2 bg-blue-500 text-white rounded">Back to Main</button>

      <h2>User List</h2>
      <table className="min-w-full divide-y divide-gray-300">
        <thead>
          <tr>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Username</th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Display Name</th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Domain</th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Enabled</th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">RDP Enabled</th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {users.map((user) => (
            <tr key={user.username} className="hover:bg-gray-50">
              <td className="px-6 py-4 whitespace-nowrap">{user.username}</td>
              <td className="px-6 py-4 whitespace-nowrap">{user.displayName}</td>
              <td className="px-6 py-4 whitespace-nowrap">{user.domain}</td>
              <td className="px-6 py-4 whitespace-nowrap">{user.isEnabled ? 'Yes' : 'No'}</td>
              <td className="px-6 py-4 whitespace-nowrap">{user.isRdpEnabled ? 'Yes' : 'No'}</td>
              <td className="px-6 py-4 whitespace-nowrap">
                {user.isEnabled ? (
                  <Button onClick={() => disableUser(user.username)}>Disable</Button>
                ) : (
                  <Button onClick={() => enableUser(user.username)}>Enable</Button>
                )}
                <Button onClick={() => resetPassword(user.username)}>Reset Password</Button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h2>Add New User</h2>
      <div className="flex flex-col space-y-4">
        <input type="text" placeholder="Username" value={username} onChange={(e: ChangeEvent<HTMLInputElement>) => setUsername(e.target.value)} className="px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500" />
        <input type="text" placeholder="Display Name" value={displayName} onChange={(e: ChangeEvent<HTMLInputElement>) => setDisplayName(e.target.value)} className="px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500" />
        <input type="text" placeholder="Domain" value={domain} onChange={(e: ChangeEvent<HTMLInputElement>) => setDomain(e.target.value)} className="px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500" />
        <Label htmlFor="isEnabled">
          Enabled
          <Switch checked={isEnabled} onChange={() => setIsEnabled(!isEnabled)} id="isEnabled" />
        </Label>
        <Label htmlFor="isRdpEnabled">
          RDP Enabled
          <Switch checked={isRdpEnabled} onChange={() => setIsRdpEnabled(!isRdpEnabled)} id="isRdpEnabled" />
        </Label>
        <button onClick={addUser} className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600">Add User</button>
      </div>
    </div>
  );
};

export default UserManagement;