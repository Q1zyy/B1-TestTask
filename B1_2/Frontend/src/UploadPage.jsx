import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

function UploadPage() {
  const [file, setFile] = useState(null);
  const [uploadedFiles, setUploadedFiles] = useState([]);

  useEffect(() => {
    fetchFiles();
  }, []);

  const fetchFiles = async () => {
    const res = await axios.get('https://localhost:7117/api/files');
    setUploadedFiles(res.data);
  };

  const handleUpload = async (e) => {
    e.preventDefault();
    if (!file) {
        return;
    }

    const formData = new FormData();
    formData.append('formFile', file);

    await axios.post('https://localhost:7117/api/files/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });

    setFile(null);
    fetchFiles();
  };

  return (
    <div className="container mt-5">
      <h2>Загрузка файла</h2>
      <form onSubmit={handleUpload} className="mb-4">
        <input
          type="file"
          className="form-control mb-2"
          onChange={(e) => setFile(e.target.files[0])}
        />
        <button className="btn btn-primary">Загрузить</button>
      </form>

      <h3>Загруженные файлы</h3>
      <ul className="list-group">
        {uploadedFiles.map((f) => (
          <li key={f.id} className="list-group-item d-flex justify-content-between align-items-center">
            {f.fileName}
            <Link to={`/files/${f.id}`} className="btn btn-sm btn-outline-primary">
              Просмотр
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default UploadPage;
