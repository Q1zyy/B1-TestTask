import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';

function FileViewPage() {
  const { fileId } = useParams();
  const navigate = useNavigate(); 
  const [report, setReport] = useState(null);

  useEffect(() => {
    fetchReport();
  }, [fileId]);

  const fetchReport = async () => {
    const res = await axios.get(`https://localhost:7117/api/files/${fileId}`);
    setReport(res.data);
  };

  const downloadMarkdown = async () => {
    const res = await axios.get(`https://localhost:7117/api/files/${fileId}/md`, { responseType: 'blob' });
    const url = window.URL.createObjectURL(new Blob([res.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `${report.fileName}_export.md`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  };

  if (!report) {
    return <div className="container mt-5">Загрузка...</div>;
  }

  return (
    <div className="container mt-5">
      <button className="btn btn-primary mb-3" onClick={() => navigate('/')}>
        На главную
      </button>

      <h2>{report.fileName}</h2>
      <p><strong>Банк:</strong> {report.bankName}</p>
      <p>
        <strong>Период:</strong> {new Date(report.periodStart).toLocaleDateString()} - {new Date(report.periodEnd).toLocaleDateString()}
      </p>
      <button className="btn btn-success mb-3" onClick={downloadMarkdown}>Скачать Markdown</button>

      <div className="table-responsive">
        <table className="table table-bordered table-sm">
          <thead>
            <tr>
              <th>Б/сч</th>
              <th colSpan={2}>Входящее сальдо</th>
              <th colSpan={2}>Обороты</th>
              <th colSpan={2}>Исходящее сальдо</th>
            </tr>
            <tr>
              <th></th>
              <th>Актив</th>
              <th>Пассив</th>
              <th>Дебет</th>
              <th>Кредит</th>
              <th>Актив</th>
              <th>Пассив</th>
            </tr>
          </thead>
          <tbody>
            {report.classes.map((cls, idx) => (
              <React.Fragment key={idx}>
                <tr>
                  <td colSpan={7} className="fw-bold text-center bg-light">
                    КЛАСС {cls.code} {cls.name}
                  </td>
                </tr>

                {cls.accounts.map((acc, i) => {
                  const isBold =
                    acc.code.length === 2 ||
                    acc.code.toUpperCase() === 'ПО КЛАССУ' ||
                    acc.code.toUpperCase() === 'БАЛАНС';
                  return (
                    <tr key={i} className={isBold ? 'fw-bold' : ''}>
                      <td>{acc.code}</td>
                      <td>{acc.openingDebit}</td>
                      <td>{acc.openingCredit}</td>
                      <td>{acc.turnoverDebit}</td>
                      <td>{acc.turnoverCredit}</td>
                      <td>{acc.closingDebit}</td>
                      <td>{acc.closingCredit}</td>
                    </tr>
                  );
                })}
              </React.Fragment>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default FileViewPage;
