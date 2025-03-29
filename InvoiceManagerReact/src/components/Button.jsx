const Button = ({
    label,
    onClick,
    type = "button",
    variant = "primary",
    size = "small",
    className = "",
    disabled = false,
    loading = false,
  }) => {
    const baseClasses =
      "w-fit rounded-full transition-all flex items-center justify-center font-medium";
  
    const sizeClasses = {
      small: "py-2 px-4 text-sm",
      medium: "py-3 px-6 text-base",
    };
  
    const variantClasses = {
      primary:
        disabled || loading
          ? "bg-blue-300 text-white opacity-50 cursor-not-allowed"
          : "bg-blue-600 hover:bg-blue-700 text-white",
      secondary:
        disabled || loading
          ? "bg-gray-100 text-gray-400 border border-gray-300 cursor-not-allowed"
          : "bg-white text-blue-600 border border-blue-600 hover:bg-blue-50",
    };
  
    return (
      <button
        onClick={onClick}
        type={type}
        className={`${baseClasses} ${variantClasses[variant]} ${className} ${sizeClasses[size]}`}
        disabled={disabled || loading}
      >
        {loading ? (
          <>
            <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></div>
            Cargando...
          </>
        ) : (
          label
        )}
      </button>
    );
  };
  
  export default Button;
  