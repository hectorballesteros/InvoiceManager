import { Routes, Route, Navigate } from 'react-router-dom';
import Sidebar from './layouts/Sidebar';
import Login from './pages/Login';
import Home from './pages/Home';
import PrivateRoute from './utils/PrivateRoute';
import PublicRoute from './utils/PublicRoute';
import { useAuth } from './contexts/AuthContext';

function App() {
  const { isAuthenticated } = useAuth();

  return (
    <div className="flex h-screen">
      {isAuthenticated && <Sidebar />}

      <div className={`flex-grow ${isAuthenticated ? 'sm:ml-64' : ''}`}>
        <Routes>
          <Route
            path="/login"
            element={
              <PublicRoute>
                <Login />
              </PublicRoute>
            }
          />
          <Route
            path="/home"
            element={
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            }
          />
          <Route
            path="*"
            element={
              <Navigate to={isAuthenticated ? '/home' : '/login'} />
            }
          />
        </Routes>
      </div>
    </div>
  );
}

export default App;

