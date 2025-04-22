import Link from 'next/link';
import { Button } from '@heroicons/react/24/outline';

export default function Home() {
  return (
    <div className="container mx-auto p-4">
      <h1 className="text-3xl font-bold mb-4">Welcome to the Control Panel</h1>
      <Link href="/users">
        <Button className="px-4 py-2 bg-blue-500 text-white rounded">
          User Management
        </Button>
      </Link>
    </div>
  );
}