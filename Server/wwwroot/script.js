const imageInput = document.getElementById('imageInput');
const previewSection = document.getElementById('previewSection');
const previewImage = document.getElementById('previewImage');
const applyFilterButton = document.getElementById('applyFilterButton');
const sendButton = document.getElementById('sendButton');
const downloadLink = document.getElementById('downloadLink');
const statusMessage = document.getElementById('statusMessage');

let filterActive = false;

imageInput.addEventListener('change', handleImageSelection);

function handleImageSelection() {
  const file = imageInput.files?.[0];
  if (!file) {
    return;
  }

  const url = URL.createObjectURL(file);
  previewImage.src = url;
  previewImage.classList.remove('grayscale');
  filterActive = false;
  applyFilterButton.textContent = 'Aplicar filtro cinza';
  previewSection.classList.remove('hidden');
}

applyFilterButton.addEventListener('click', () => {
  if (!previewImage.src) return;
  filterActive = !filterActive;
  previewImage.classList.toggle('grayscale', filterActive);
  applyFilterButton.textContent = filterActive ? 'Remover filtro' : 'Aplicar filtro cinza';
});

const uploadBox = document.querySelector('.upload-box');
uploadBox.addEventListener('dragover', (event) => {
  event.preventDefault();
  uploadBox.classList.add('drag-over');
});

uploadBox.addEventListener('dragleave', () => {
  uploadBox.classList.remove('drag-over');
});

uploadBox.addEventListener('drop', (event) => {
  event.preventDefault();
  uploadBox.classList.remove('drag-over');
  const files = event.dataTransfer?.files;
  if (!files?.length) return;
  imageInput.files = files;
  handleImageSelection();
});

// Envia a imagem selecionada para o servidor para processar
sendButton.addEventListener('click', async () => {
  const file = imageInput.files?.[0];
  if (!file) {
    statusMessage.textContent = 'Nenhuma imagem selecionada.';
    return;
  }

  statusMessage.textContent = 'Enviando imagem para o servidor...';
  sendButton.disabled = true;
  applyFilterButton.disabled = true;
  downloadLink.classList.add('disabled');

  try {
    const form = new FormData();
    form.append('file', file, file.name);

    // Altere a URL abaixo para o endpoint real do backend quando disponível
    const res = await fetch('/api/apply-grayscale', {
      method: 'POST',
      body: form,
    });

    if (!res.ok) {
      throw new Error(`Resposta do servidor: ${res.status} ${res.statusText}`);
    }

    const blob = await res.blob();
    const url = URL.createObjectURL(blob);
    downloadLink.href = url;
    downloadLink.classList.remove('disabled');
    statusMessage.textContent = 'Pronto — clique em Baixar resultado para salvar.';

    // Opcional: iniciar download automaticamente
    // downloadLink.click();
  } catch (err) {
    console.error(err);
    statusMessage.textContent = 'Falha ao enviar a imagem. Verifique o backend.';
  } finally {
    sendButton.disabled = false;
    applyFilterButton.disabled = false;
  }
});
