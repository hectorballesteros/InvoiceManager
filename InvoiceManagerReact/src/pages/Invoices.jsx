import React, { useEffect, useState, useRef } from 'react';
import Card from '../components/Card';
import Table from '../components/Table';
import Button from "../components/Button";
import InvoiceDetailModal from "../components/modals/InvoiceDetailModal";
import toast from 'react-hot-toast';
import axios from 'axios';

const Invoices = () => {
  const [invoices, setInvoices] = useState([]);
  const [importing, setImporting] = useState(false);
  const [loading, setLoading] = useState(false);
  const [selectedInvoice, setSelectedInvoice] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const fileInputRef = useRef();

  const openInvoiceModal = (invoice) => {
    setSelectedInvoice(invoice);
    setIsModalOpen(true);
  };

  useEffect(() => {
    document.title = 'Facturas - Invoice Manager';
    fetchData();
  }, []);

  // Filtros
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedFile, setSelectedFile] = useState(null);
  const [searching, setSearching] = useState(false);
  const [invoiceStatus, setInvoiceStatus] = useState(null);
  const [paymentStatus, setPaymentStatus] = useState(null);

  const fetchData = async () => {
    setLoading(true);
    try {
      let url = '/invoice';
      const filters = [];
      if (invoiceStatus) filters.push(`invoiceStatus=${invoiceStatus}`);
      if (paymentStatus) filters.push(`paymentStatus=${paymentStatus}`);
      if (filters.length) url = `/invoice/status?${filters.join('&')}`;

      const res = await axios.get(url);
      setInvoices(res.data.data.reverse());
    } catch (err) {
      console.error('Error al obtener facturas:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchInvoiceByNumber = async () => {
    if (!searchTerm) {
      fetchData();
      return;
    }

    setLoading(true);
    setSearching(true);
    try {
      const res = await axios.get(`/invoice/${searchTerm}`);
      setInvoices([res.data.data]);
    } catch (err) {
      setInvoices([]);
      console.error("Factura no encontrada o error:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [invoiceStatus, paymentStatus]);

  const invoiceOptions = ["Issued", "Partial", "Cancelled"];
  const paymentOptions = ["Paid", "Pending", "Overdue"];

  const clearFilter = (type) => {
    if (type === 'invoice') setInvoiceStatus(null);
    if (type === 'payment') setPaymentStatus(null);
  };

  const handleImport = async () => {
    if (!selectedFile) return;

    const formData = new FormData();
    formData.append('file', selectedFile);
    setImporting(true);

    try {
      const res = await axios.post('/invoice/import', formData);
      const { success, message, errors } = res.data;

      if (success) {
        toast.success(message, { duration: 8000 });
        fetchData();
        setSelectedFile(null);
        fileInputRef.current.value = null;

        if (errors && errors.length > 0) {
          const errorSummary = errors.slice(0, 3).join('\n');
          toast.error(`Algunas facturas fueron omitidas:\n${errorSummary}`, {
            duration: 10000,
            style: { whiteSpace: 'pre-wrap' },
          });
        }
      } else {
        toast.error(message || 'Error al importar las facturas');
      }
    } catch (err) {
      console.error('Error al importar facturas:', err);
      toast.error(err.response.data.message || 'Error al importar las facturas');
    } finally {
      setImporting(false);
    }
  };

  const columns = [
    { header: "N°", accessorKey: "invoice_number", enableSorting: true },
    { header: "Cliente", accessorKey: "customer.customer_name", enableSorting: false },
    {
      header: "Monto",
      accessorKey: "total_amount",
      enableSorting: false,
      cell: ({ row }) => `$${row.original.total_amount.toLocaleString()}`
    },
    {
      header: "Fecha",
      accessorKey: "invoice_date",
      enableSorting: true,
      cell: ({ row }) => new Date(row.original.invoice_date).toLocaleDateString("es-CL"),
    },
    { header: "Estado de factura", accessorKey: "invoice_status", enableSorting: false },
    { header: "Estado de pago", accessorKey: "payment_status", enableSorting: false },
    {
      header: " ",
      cell: ({ row }) => (
        <div className="flex items-center gap-3">
          <button onClick={() => openInvoiceModal(row.original)} className="text-gray-500 hover:text-gray-700">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" stroke="currentColor" className="size-6">
              <path strokeLinecap="round" strokeLinejoin="round" d="M9 12h3.75M9 15h3.75M9 18h3.75m3 .75H18a2.25 2.25 0 0 0 2.25-2.25V6.108c0-1.135-.845-2.098-1.976-2.192a48.424 48.424 0 0 0-1.123-.08m-5.801 0c-.065.21-.1.433-.1.664 0 .414.336.75.75.75h4.5a.75.75 0 0 0 .75-.75 2.25 2.25 0 0 0-.1-.664m-5.8 0A2.251 2.251 0 0 1 13.5 2.25H15c1.012 0 1.867.668 2.15 1.586m-5.8 0c-.376.023-.75.05-1.124.08C9.095 4.01 8.25 4.973 8.25 6.108V8.25m0 0H4.875c-.621 0-1.125.504-1.125 1.125v11.25c0 .621.504 1.125 1.125 1.125h9.75c.621 0 1.125-.504 1.125-1.125V9.375c0-.621-.504-1.125-1.125-1.125H8.25Z" />
            </svg>
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="p-4 sm:p-6">

      {/* Importar Facturas */}
      <div className="grid grid-cols-1 sm:grid-cols-3 my-4 px-4">
            <Card
                title="Importar facturas"
                icon={
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" stroke="currentColor" className="w-6 h-6">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M3 16.5v2.25A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75V16.5m-13.5-9L12 3m0 0l4.5 4.5M12 3v13.5" />
                </svg>
                }
            >
                <div className="flex flex-col h-full">
                <p className="text-sm text-gray-600">Importa las facturas en formato JSON.</p>
                <p className="text-xs text-blue-500">Descarga un archivo de ejemplo.</p>

                <div className="mt-4">
                    <input
                    ref={fileInputRef}
                    type="file"
                    accept=".json"
                    onChange={(e) => setSelectedFile(e.target.files[0])}
                    className="w-full text-slate-500 font-medium text-sm bg-white border file:cursor-pointer cursor-pointer file:border-0 file:py-3 file:px-4 file:mr-4 file:bg-gray-100 file:hover:bg-gray-200 file:text-slate-500 rounded"
                    />
                    <p className="text-xs text-slate-500 mt-2">Solo archivos .json</p>

                    <div className="mt-3 float-end">
                    <Button
                        label="Importar"
                        size="small"
                        disabled={!selectedFile || importing}
                        loading={importing}
                        onClick={handleImport}
                    >
                        Importar
                    </Button>
                    </div>
                </div>
                </div>
            </Card>
      </div>

      {/* Listado de Facturas */}
      <div className="col-span-1 md:col-span-3">
        <Card title="Listado de Facturas" className="overflow-x-auto">
          {/* Filtros */}
          <div className="flex flex-col lg:flex-row justify-between gap-4 items-start mb-4">
            <div className='flex flex-wrap gap-3'>
              {invoiceStatus && (
                <span className="bg-purple-100 text-purple-700 px-2 py-1 rounded-full text-xs flex items-center gap-2">
                  Factura: {invoiceStatus}
                  <button onClick={() => clearFilter("invoice")}>✕</button>
                </span>
              )}
              {paymentStatus && (
                <span className="bg-blue-100 text-blue-700 px-2 py-1 rounded-full text-xs flex items-center gap-2">
                  Pago: {paymentStatus}
                  <button onClick={() => clearFilter("payment")}>✕</button>
                </span>
              )}
            </div>

            <div className='flex flex-wrap gap-4 w-full lg:w-auto'>
              <select
                value={invoiceStatus || ""}
                onChange={(e) => setInvoiceStatus(e.target.value || null)}
                className="border rounded px-3 py-1 text-sm"
              >
                <option value="">Estado de Factura</option>
                {invoiceOptions.map((option) => (
                  <option key={option} value={option}>{option}</option>
                ))}
              </select>
              <select
                value={paymentStatus || ""}
                onChange={(e) => setPaymentStatus(e.target.value || null)}
                className="border rounded px-3 py-1 text-sm"
              >
                <option value="">Estado de Pago</option>
                {paymentOptions.map((option) => (
                  <option key={option} value={option}>{option}</option>
                ))}
              </select>
              <input
                type="number"
                placeholder="N° de factura"
                className="border px-3 py-1 rounded text-sm"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') handleSearchInvoiceByNumber();
                }}
              />
            </div>
          </div>

          {/* Tabla con scroll horizontal */}
          <div className="overflow-x-auto">
            <Table data={invoices} columns={columns} pagination loading={loading} />
          </div>
        </Card>
      </div>

      <InvoiceDetailModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        invoice={selectedInvoice}
      />
    </div>
  );
};

export default Invoices;
