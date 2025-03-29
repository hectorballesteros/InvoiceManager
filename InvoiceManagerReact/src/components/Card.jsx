import React from "react";

const Card = ({ title, icon, children, className = "" }) => {
  return (
    <div className={`bg-white border border-slate-800 rounded-[20px] p-4 flex flex-col justify-between h-full shadow w-full ${className}`}>     
     {/* Header */}
      <div className="flex items-center gap-3 min-h-[40px] mb-2">
        {icon && <div className="text-indigo-600">{icon}</div>}
        {title && <h2 className="text-lg font-medium text-gray-800">{title}</h2>}
      </div>

      {/* Body */}
      <div className="">{children}</div>
    </div>
  );
};

export default Card;
