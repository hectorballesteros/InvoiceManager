import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import logo from "../assets/logo-finix.png";

const Sidebar = ({ user = {}, routes = [], onLogout }) => {
  const location = useLocation();
  const [isOpen, setIsOpen] = useState(false);
  const userInitial = user?.name?.charAt(0).toUpperCase() || "?";

  const toggleSidebar = () => setIsOpen(!isOpen);
  const isActive = (path) => location.pathname === path;

  return (
    <>
      {/* Botón móvil */}
      <button
        onClick={toggleSidebar}
        className="fixed top-4 left-4 z-50 p-2 text-purple-900 rounded-lg sm:hidden hover:bg-purple-900 hover:text-white"
      >
        <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 20 20">
          <path
            clipRule="evenodd"
            fillRule="evenodd"
            d="M2 4.75A.75.75 0 012.75 4h14.5a.75.75 0 010 1.5H2.75A.75.75 0 012 4.75zm0 
             10.5a.75.75 0 01.75-.75h7.5a.75.75 0 010 1.5h-7.5a.75.75 
             0 01-.75-.75zM2 10a.75.75 0 01.75-.75h14.5a.75.75 
             0 010 1.5H2.75A.75.75 0 012 10z"
          />
        </svg>
      </button>

      {/* Sidebar */}
      <aside
        className={`fixed top-0 left-0 z-40 w-64 h-screen bg-slate-800 border-r-8 border-slate-700 transition-transform ${
          isOpen ? "translate-x-0" : "-translate-x-full"
        } sm:translate-x-0`}
      >
        {/* Logo */}
        <div className="flex items-center justify-center py-4">
          <img src={logo} alt="Logo" className="h-16 w-auto" />
        </div>

        {/* Usuario + logout */}
        <div className="flex items-center justify-center space-x-2 px-4 mb-6">
          <div className="size-12 bg-green-500 text-white font-bold flex items-center justify-center rounded-full">
            {userInitial}
          </div>
          <div className="w-px h-9 bg-gray-500"></div>
          <div
            onClick={onLogout}
            className="size-10 bg-gray-200 hover:bg-gray-300 text-black font-bold flex items-center justify-center rounded-full cursor-pointer"
            title="Cerrar sesión"
          >
            ⎋
          </div>
        </div>

        {/* Rutas */}
        <nav className="flex flex-col space-y-4 px-4">
          {routes.map(({ path, label, icon, visible }) =>
            visible === false ? null : (
              <Link
                key={path}
                to={path}
                className={`flex items-center gap-3 px-4 py-2 rounded-lg ${
                  isActive(path)
                    ? "bg-slate-700 text-white"
                    : "text-gray-400 hover:text-gray-300"
                }`}
              >
                {icon && <span>{icon}</span>}
                <span>{label}</span>
              </Link>
            )
          )}
        </nav>
      </aside>
    </>
  );
};

export default Sidebar;
