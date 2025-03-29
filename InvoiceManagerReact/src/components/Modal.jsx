import React from "react";

const Modal = ({ isOpen, onClose, children, title, disableBackdrop = false }) => {
  if (!isOpen) return null;

  return (
    <div className={`fixed inset-0 rounded-lg flex items-center justify-center ${disableBackdrop ? "pointer-events-none" : "z-50 bg-black bg-opacity-50"}`}>
      <div className="bg-white rounded-lg shadow-xl w-full max-w-xl z-50 pointer-events-auto flex flex-col max-h-[90vh]">
        
        {/* Encabezado fijo */}
        <div className="flex justify-between rounded-lg items-center border-b px-6 py-4 bg-white z-10">
          <h2 className="text-lg font-semibold text-gray-900">{title}</h2>
          <button onClick={onClose} className="text-gray-500 hover:text-gray-700">
            ✖
          </button>
        </div>

        {/* Contenido scrolleable */}
        <div className="overflow-y-auto px-6 py-4">
          {children}
        </div>
      </div>
    </div>
  );
};

export default Modal;
