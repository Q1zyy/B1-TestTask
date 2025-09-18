import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import UploadPage from './UploadPage';
import FileViewPage from './FileViewPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<UploadPage />} />
        <Route path="/files/:fileId" element={<FileViewPage />} />
      </Routes>
    </Router>
  );
}

export default App;