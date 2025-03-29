import React from 'react';

const InputField = ({
  label,
  placeholder,
  name,
  value,
  onChange,
  type = "text",
  error,
  ...props
}) => {
  return (
    <div className="flex flex-col w-full">
      {/* Etiqueta */}
      {label && (
        <label className="block text-sm text-gray-700 mb-1">
          {label}
        </label>
      )}

      {/* Input */}
      <input
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className={`w-full px-4 py-2 rounded-full border transition focus:outline-none focus:ring-2
          ${error
            ? "border-red-500 text-red-600 placeholder-red-400 focus:ring-red-400"
            : "border-gray-300 text-gray-800 placeholder-gray-400 focus:ring-blue-400"}
        `}
        {...props}
      />

      {/* Mensaje de error */}
      {error && (
        <p className="text-sm text-red-500 mt-1">
          {error}
        </p>
      )}
    </div>
  );
};

export default InputField;
