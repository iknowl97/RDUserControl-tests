import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@heroicons/react/20/solid';
import {
  Button,
  Input,
  Label,
  Switch,
} from '@heroicons/react/20/solid';

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
      <Button onClick={() => router.push('/')}>Back to Main</Button>

      <h2>User List</h2>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Username</TableHead>
            <TableHead>Display Name</TableHead>
            <TableHead>Domain</TableHead>
            <TableHead>Enabled</TableHead>
            <TableHead>RDP Enabled</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {users.map((user) => (
            <TableRow key={user.username}>
              <TableCell>{user.username}</TableCell>
              <TableCell>{user.displayName}</TableCell>
              <TableCell>{user.domain}</TableCell>
              <TableCell>{user.isEnabled ? 'Yes' : 'No'}</TableCell>
              <TableCell>{user.isRdpEnabled ? 'Yes' : 'No'}</TableCell>
              <TableCell>
                {user.isEnabled ? (
                  <Button onClick={() => disableUser(user.username)}>Disable</Button>
                ) : (
                  <Button onClick={() => enableUser(user.username)}>Enable</Button>
                )}
                <Button onClick={() => resetPassword(user.username)}>Reset Password</Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>

      <h2>Add New User</h2>
      <div className="flex flex-col space-y-4">
        <Input type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
        <Input type="text" placeholder="Display Name" value={displayName} onChange={(e) => setDisplayName(e.target.value)} />
        <Input type="text" placeholder="Domain" value={domain} onChange={(e) => setDomain(e.target.value)} />
        <Label htmlFor="isEnabled">
          Enabled
          <Switch checked={isEnabled} onChange={() => setIsEnabled(!isEnabled)} id="isEnabled" />
        </Label>
        <Label htmlFor="isRdpEnabled">
          RDP Enabled
          <Switch checked={isRdpEnabled} onChange={() => setIsRdpEnabled(!isRdpEnabled)} id="isRdpEnabled" />
        </Label>
        <Button onClick={addUser}>Add User</Button>
      </div>
    </div>
  );
};

export default UserManagement;