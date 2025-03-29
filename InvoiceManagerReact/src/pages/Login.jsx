import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import InputField from '../components/InputField';
import Button from '../components/Button';
import rightImage from '../assets/Of1.png';
import logo from '../assets/logo-finix.png';
import { useAuth } from '../contexts/AuthContext';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();
  useEffect(() => {
    document.title = 'Login - Invoice Manager';
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setErrors({}); // limpiar errores anteriores

    const newErrors = {};

    if (!username.trim()) newErrors.username = 'El usuario es obligatorio';
    if (!password.trim()) newErrors.password = 'La contraseña es obligatoria';

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      setLoading(false);
      return;
    }

    try {
      // login del contexto
      await login({ username, password });
      navigate('/inicio'); // solo si login fue exitoso
    } catch (err) {
      setErrors({ general: err.message || 'Credenciales incorrectas' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="h-screen w-screen grid md:grid-cols-5">
      {/* Sección derecha: imagen decorativa */}
      <div className="hidden md:col-span-3 md:flex justify-center items-center bg-gray-50">
        <img
          src={rightImage}
          alt="Imagen decorativa"
          className="w-full h-full object-cover"
        />
      </div>

      {/* Sección izquierda: formulario */}
      <div className="col-span-2 flex flex-col justify-center items-center bg-white p-10 w-full h-full">
        <img src={logo} alt="Logo" className="h-40" />
        <div className="mb-6 text-center">
          <h1 className="text-2xl font-semibold text-gray-700">Iniciar sesión</h1>
        </div>

        <form onSubmit={handleSubmit} className="w-full max-w-sm flex flex-col gap-4">
          <InputField
            label="Usuario o correo electrónico"
            placeholder="nombre@mail.com"
            name="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            error={errors.username}
          />

          <InputField
            label="Contraseña"
            type="password"
            placeholder="********"
            name="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            error={errors.password}
          />

          <Button
            type="submit"
            label="Ingresar"
            variant="primary"
            size="medium"
            className="w-full"
            loading={loading}
          />

          {errors.general && (
            <p className="text-sm text-red-500 text-center mt-2">
              {errors.general}
            </p>
          )}
        </form>
      </div>
    </div>
  );
};

export default Login;
