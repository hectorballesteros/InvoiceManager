import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import logo from "../assets/logo-finix.png";
import { useAuth } from "../contexts/AuthContext";

const Sidebar = () => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const [isOpen, setIsOpen] = useState(false);
  const userInitial = user?.username?.charAt(0).toUpperCase();
  const toggleSidebar = () => setIsOpen(!isOpen);
  const isActive = (path) => location.pathname === path;

  const routes = [
    {
      path: "/facturas",
      label: "Facturas",
      icon: (
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-6">
          <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m3.75 9v7.5m2.25-6.466a9.016 9.016 0 0 0-3.461-.203c-.536.072-.974.478-1.021 1.017a4.559 4.559 0 0 0-.018.402c0 .464.336.844.775.994l2.95 1.012c.44.15.775.53.775.994 0 .136-.006.27-.018.402-.047.539-.485.945-1.021 1.017a9.077 9.077 0 0 1-3.461-.203M10.5 2.25H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" />
        </svg>
      ),
    },
  ];

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
        <div className="flex items-center justify-center space-x-2 px-4">
            <div className="size-12 bg-green-500 text-white font-bold flex items-center justify-center rounded-full">
                { userInitial }
            </div>
            <div className="w-px h-9 bg-gray-500"></div>
            <div className="size-10 bg-gray-200 text-black hover:bg-gray-300 font-bold flex items-center justify-center rounded-full cursor-pointer" onClick={logout}>
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
                <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0 0 13.5 3h-6a2.25 2.25 0 0 0-2.25 2.25v13.5A2.25 2.25 0 0 0 7.5 21h6a2.25 2.25 0 0 0 2.25-2.25V15M12 9l-3 3m0 0 3 3m-3-3h12.75" />
                </svg>
            </div>
        </div>

        {/* Rutas */}
        <nav className="flex flex-col space-y-4 px-4 my-16">
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
