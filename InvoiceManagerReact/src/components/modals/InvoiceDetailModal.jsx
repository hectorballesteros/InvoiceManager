import React, { useState, useEffect } from 'react';
import axios from 'axios';
import toast from 'react-hot-toast';
import Modal from '../Modal';
import Button from '../Button';
import Card from '../Card';

const InvoiceDetailModal = ({ isOpen, onClose, invoice }) => {
  const [creditAmount, setCreditAmount] = useState("");
  const [saving, setSaving] = useState(false);
  const [localInvoice, setLocalInvoice] = useState(null);

  useEffect(() => {
    if (invoice) setLocalInvoice({ ...invoice });
  }, [invoice]);

  if (!localInvoice) return null;

  const handleAddCreditNote = async () => {
    const amount = parseFloat(creditAmount);

    if (!amount || isNaN(amount) || amount <= 0) {
      toast.error("Ingresa un monto válido para la nota de crédito");
      return;
    }

    setSaving(true);
    try {
      const res = await axios.post(`/invoice/${localInvoice.invoice_number}/credit-note`, {
        creditNoteAmount: amount
      });

      toast.success(res.data.message || "Nota de crédito agregada");

      // Traer factura actualizada
      const updated = await axios.get(`/invoice/${localInvoice.invoice_number}`);
      setLocalInvoice(updated.data.data); // Actualizar con nueva info

      setCreditAmount("");
    } catch (error) {
      const msg = error.response?.data?.message || "Error al agregar nota de crédito";
      const details = error.response?.data?.errors?.join("\n");
      toast.error(`${msg}${details ? `\n${details}` : ''}`, {
        style: { whiteSpace: 'pre-wrap' }
      });
    } finally {
      setSaving(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={`Factura N° ${localInvoice.invoice_number}`}>
      <div className="space-y-4 text-sm text-gray-700">

        {/* Información completa de factura */}
        <Card title="Información de Factura">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 mb-4">
            <p><strong>Fecha:</strong> {new Date(localInvoice.invoice_date).toLocaleDateString("es-CL")}</p>
            <p><strong>Estado:</strong> {localInvoice.invoice_status}</p>
            <p><strong>Estado de Pago:</strong> {localInvoice.payment_status}</p>
            <p><strong>Total:</strong> ${localInvoice.total_amount.toLocaleString()}</p>
            <p><strong>Fecha Vencimiento:</strong> {new Date(localInvoice.payment_due_date).toLocaleDateString("es-CL")}</p>
            <p><strong>Días hasta vencimiento:</strong> {localInvoice.days_to_due} días</p>
          </div>

          {/* Cliente */}
          <div className="border-t pt-4 mt-4">
            <h3 className="text-sm font-semibold text-gray-800 mb-2">Cliente</h3>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
              <p><strong>Nombre:</strong> {localInvoice.customer?.customer_name}</p>
              <p><strong>Email:</strong> {localInvoice.customer?.customer_email}</p>
              <p><strong>RUN:</strong> {localInvoice.customer?.customer_run}</p>
            </div>
          </div>

          {/* Pago */}
          {localInvoice.invoice_payment && (
            <div className="border-t pt-4 mt-4">
              <h3 className="text-sm font-semibold text-gray-800 mb-2">Pago</h3>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                <p><strong>Método:</strong> {localInvoice.invoice_payment?.payment_method}</p>
                <p><strong>Fecha de pago:</strong> {new Date(localInvoice.invoice_payment?.payment_date).toLocaleDateString("es-CL")}</p>
              </div>
            </div>
          )}
        </Card>

        {/* Notas de crédito */}
        <Card title="Notas de Crédito">
          {localInvoice.invoice_credit_note?.length > 0 ? (
            <ul className="list-disc list-inside text-sm mb-4">
              {localInvoice.invoice_credit_note.map((note) => (
                <li key={note.id}>
                  Nota #{note.credit_note_number} por ${note.credit_note_amount.toLocaleString()}
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-sm text-gray-500 mb-4">Esta factura aún no tiene notas de crédito.</p>
          )}

          {/* Formulario para agregar nota */}
          <div className="border-t pt-4 mt-2">
            <h3 className="text-sm font-semibold text-gray-800 mb-2">Agregar nueva nota de crédito</h3>
            <div className="flex flex-col justify-between sm:flex-row gap-2">
              <input
                type="number"
                min="0"
                step="0.01"
                value={creditAmount}
                onChange={(e) => setCreditAmount(e.target.value)}
                placeholder="Monto"
                className="border px-3 py-1 rounded text-sm w-full sm:max-w-xs"
              />
              <Button
                label="Guardar"
                size="small"
                loading={saving}
                onClick={handleAddCreditNote}
              >
                Guardar
              </Button>
            </div>
          </div>
        </Card>

        {/* Detalle de productos */}
        <Card title="Detalle de Productos">
          <div className="space-y-2">
            {localInvoice.invoice_detail?.map((item, index) => (
              <div key={item.id} className="border rounded p-3 bg-gray-50">
                <p><strong>{index + 1}. {item.product_name}</strong></p>
                <p>Precio unitario: ${item.unit_price.toLocaleString()}</p>
                <p>Cantidad: {item.quantity}</p>
                <p>Subtotal: ${item.subtotal.toLocaleString()}</p>
              </div>
            ))}
          </div>
        </Card>
      </div>
    </Modal>
  );
};

export default InvoiceDetailModal;
