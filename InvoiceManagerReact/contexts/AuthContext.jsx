import { createContext, useContext, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

// Crear el contexto
const AuthContext = createContext();

// Hook personalizado
export const useAuth = () => useContext(AuthContext);

// Provider del contexto
export const AuthProvider = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null); // puede tener nombre, rol, etc.
  const navigate = useNavigate();

  useEffect(() => {
    // Verificar si hay sesión persistida
    const storedUser = localStorage.getItem("user");
    if (storedUser) {
      setUser(JSON.parse(storedUser));
      setIsAuthenticated(true);
    }
  }, []);

  const login = ({ username, password }) => {
    // Acá simulás la validación
    if (username === "admin" && password === "123") {
      const fakeUser = { name: "Admin", role: "Administrador" };
      localStorage.setItem("user", JSON.stringify(fakeUser));
      setUser(fakeUser);
      setIsAuthenticated(true);
      navigate("/home");
    } else {
      throw new Error("Credenciales inválidas");
    }
  };

  const logout = () => {
    localStorage.removeItem("user");
    setUser(null);
    setIsAuthenticated(false);
    navigate("/login");
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
