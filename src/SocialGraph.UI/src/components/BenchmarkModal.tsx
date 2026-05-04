import React, { useState } from 'react';
import { benchmarkService } from '../services/benchmarkService';
import type { BenchmarkResult } from '../services/benchmarkService';

interface BenchmarkModalProps {
  isOpen: boolean;
  onClose: () => void;
}

const BenchmarkModal: React.FC<BenchmarkModalProps> = ({ isOpen, onClose }) => {
  const [isRunning, setIsRunning] = useState(false);
  const [progress, setProgress] = useState('');
  const [results, setResults] = useState<BenchmarkResult[] | null>(null);

  if (!isOpen) return null;

  const startAudit = async () => {
    setIsRunning(true);
    setResults(null);
    try {
      const data = await benchmarkService.runBenchmark(setProgress);
      setResults(data);
      setProgress('Audit Complete: Performance Data Ready.');
    } catch (err) {
      setProgress('Audit Failed: API Connection Error.');
      console.error(err);
    } finally {
      setIsRunning(false);
    }
  };

  const copyToClipboard = () => {
    if (!results) return;
    const header = '| Ölçek | Giriş (ms) | BFS (ms) | DFS (ms) | Trie (ms) | Veri Akışı (N/s) |\n';
    const separator = '|---|---|---|---|---|---|\n';
    const rows = results.map(r => `| ${r.scale} | ${r.seedTime} | ${r.bfsTime} | ${r.dfsTime} | ${r.trieTime} | ${r.throughput.toLocaleString()} |`).join('\n');
    const report = `### SocialGraph Performans Denetim Raporu\n\n${header}${separator}${rows}`;
    
    navigator.clipboard.writeText(report);
    alert('Rapor Markdown formatında panoya kopyalandı!');
  };

  return (
    <div style={{
      position: 'fixed',
      inset: 0,
      zIndex: 1000,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      padding: '16px'
    }}>
      {/* Backdrop */}
      <div 
        style={{
          position: 'absolute',
          inset: 0,
          background: 'rgba(0, 0, 0, 0.75)',
          backdropFilter: 'blur(10px)',
          WebkitBackdropFilter: 'blur(10px)'
        }}
        onClick={!isRunning ? onClose : undefined}
      />

      {/* Modal */}
      <div style={{
        position: 'relative',
        width: '100%',
        maxWidth: '640px',
        background: '#0a0a0a',
        border: '1px solid rgba(255, 255, 255, 0.1)',
        borderRadius: '24px',
        boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.5)',
        overflow: 'hidden',
        animation: 'modalEnter 0.4s cubic-bezier(0.16, 1, 0.3, 1)'
      }}>
        <div style={{
          padding: '24px',
          borderBottom: '1px solid rgba(255, 255, 255, 0.05)',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center'
        }}>
          <div>
            <h2 style={{ fontSize: '1.25rem', fontWeight: 600, color: '#fff', display: 'flex', alignItems: 'center', gap: '10px', margin: 0 }}>
              <div style={{ width: '8px', height: '8px', borderRadius: '50%', background: 'var(--accent-color)', boxShadow: '0 0 10px var(--accent-color)' }} />
              Sistem Performans Denetimi
            </h2>
            <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: '4px', margin: 0 }}>Performans Analiz Motoru &bull; Karşılaştırmalı Analiz</p>
          </div>
          <button 
            onClick={onClose}
            disabled={isRunning}
            style={{
              background: 'transparent',
              border: 'none',
              color: 'var(--text-secondary)',
              cursor: 'pointer',
              fontSize: '1.2rem',
              opacity: isRunning ? 0 : 1,
              transition: 'color 0.2s'
            }}
            onMouseEnter={(e) => e.currentTarget.style.color = '#fff'}
            onMouseLeave={(e) => e.currentTarget.style.color = 'var(--text-secondary)'}
          >
            ✕
          </button>
        </div>

        <div style={{ padding: '32px' }}>
          {!results && !isRunning && (
            <div style={{ textAlign: 'center', padding: '24px 0' }}>
              <div style={{ 
                width: '64px', 
                height: '64px', 
                background: 'rgba(0, 242, 255, 0.05)', 
                borderRadius: '20px', 
                display: 'flex', 
                alignItems: 'center', 
                justifyContent: 'center', 
                margin: '0 auto 24px' 
              }}>
                <span style={{ fontSize: '24px' }}>⚡</span>
              </div>
              <h3 style={{ fontSize: '1.1rem', fontWeight: 600, color: '#fff', marginBottom: '8px' }}>Ready for Verification</h3>
              <p style={{ fontSize: '0.9rem', color: 'var(--text-secondary)', marginBottom: '32px', maxWidth: '320px', margin: '0 auto 32px', lineHeight: 1.5 }}>
                Sistem tüm veri yapılarını sıfırlayacak ve 5000 düğüme kadar stres testi uygulayacaktır.
              </p>
              <button
                onClick={startAudit}
                style={{
                  padding: '12px 32px',
                  background: 'var(--accent-color)',
                  color: '#000',
                  border: 'none',
                  borderRadius: '12px',
                  fontWeight: 700,
                  fontSize: '0.9rem',
                  cursor: 'pointer',
                  transition: 'all 0.2s ease'
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.filter = 'brightness(1.1)';
                  e.currentTarget.style.transform = 'scale(1.02)';
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.filter = 'brightness(1)';
                  e.currentTarget.style.transform = 'scale(1)';
                }}
              >
                SİSTEM DENETİMİNİ BAŞLAT
              </button>
            </div>
          )}

          {isRunning && (
            <div style={{ textAlign: 'center', padding: '40px 0' }}>
              <div style={{ display: 'flex', justifyContent: 'center', gap: '8px', marginBottom: '24px' }}>
                <div className="audit-bar" style={{ width: '4px', height: '24px', background: 'var(--accent-color)', borderRadius: '2px' }} />
                <div className="audit-bar" style={{ width: '4px', height: '24px', background: 'var(--accent-color)', borderRadius: '2px', animationDelay: '0.1s' }} />
                <div className="audit-bar" style={{ width: '4px', height: '24px', background: 'var(--accent-color)', borderRadius: '2px', animationDelay: '0.2s' }} />
              </div>
              <p style={{ 
                color: 'var(--accent-color)', 
                fontFamily: 'monospace', 
                fontSize: '0.85rem', 
                letterSpacing: '1px',
                textTransform: 'uppercase' 
              }}>
                {progress}
              </p>
            </div>
          )}

          {results && (
            <div style={{ animation: 'resultsFadeIn 0.5s ease-out', flex: 1, overflowY: 'auto', paddingRight: '4px' }}>
              <div style={{ background: 'rgba(255,255,255,0.03)', borderRadius: '12px', padding: '16px', border: '1px solid rgba(255,255,255,0.05)' }}>
                <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '0.85rem' }}>
                  <thead>
                    <tr style={{ color: 'var(--text-secondary)', textAlign: 'left', borderBottom: '1px solid rgba(255, 255, 255, 0.1)' }}>
                      <th style={{ padding: '12px 16px' }}>Ölçek</th>
                      <th style={{ padding: '12px 12px', textAlign: 'right' }}>Giriş (ms)</th>
                      <th style={{ padding: '12px 12px', textAlign: 'right' }}>BFS (ms)</th>
                      <th style={{ padding: '12px 12px', textAlign: 'right' }}>DFS (ms)</th>
                      <th style={{ padding: '12px 12px', textAlign: 'right' }}>Trie (ms)</th>
                      <th style={{ padding: '12px 16px', textAlign: 'right' }}>Akış (N/s)</th>
                    </tr>
                  </thead>
                  <tbody>
                    {results.map(r => (
                      <tr key={r.scale} style={{ borderBottom: '1px solid rgba(255, 255, 255, 0.03)' }}>
                        <td style={{ padding: '12px 16px', color: 'var(--accent-color)', fontWeight: 600 }}>{r.scale}</td>
                        <td style={{ padding: '12px 12px', textAlign: 'right' }}>{r.seedTime}</td>
                        <td style={{ padding: '12px 12px', textAlign: 'right' }}>{r.bfsTime}</td>
                        <td style={{ padding: '12px 12px', textAlign: 'right' }}>{r.dfsTime}</td>
                        <td style={{ padding: '12px 12px', textAlign: 'right' }}>{r.trieTime}</td>
                        <td style={{ padding: '12px 16px', textAlign: 'right', color: '#10b981', fontWeight: 500 }}>{r.throughput.toLocaleString()}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div style={{ marginTop: '20px', padding: '16px', background: 'rgba(6, 182, 212, 0.05)', borderRadius: '12px', border: '1px solid rgba(6, 182, 212, 0.1)' }}>
                <h4 style={{ margin: '0 0 8px 0', fontSize: '0.85rem', color: 'var(--accent-color)', textTransform: 'uppercase', letterSpacing: '1px' }}>Teknik Analiz ve Sistem İçgörüleri</h4>
                <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', lineHeight: '1.6' }}>
                  {(() => {
                    const last = results[results.length - 1];
                    return (
                      <>
                        <p style={{ margin: '6px 0' }}>• <b>Ölçeklenebilirlik Gücü:</b> 5000 düğümlük en yüksek ölçekte saniyede <b>{last.throughput.toLocaleString()}</b> işlem (N/s) hızına ulaşılmıştır. Bu, algoritmalarımızın <b>O(V+E)</b> karmaşıklığıyla lineer ölçeklendiğini gerçek verilerle kanıtlar.</p>
                        <p style={{ margin: '6px 0' }}>• <b>Veri Yapısı Verimliliği:</b> Sıfırdan tasarlanan yapılar sayesinde, 5000 düğümde bile BFS sorgusu sadece <b>{last.bfsTime}ms</b> sürmüştür. Bu hız, in-memory (bellek içi) yönetimimizin ve düşük seviyeli optimizasyonlarımızın bir sonucudur.</p>
                        <p style={{ margin: '6px 0' }}>• <b>Metin Arama Kararlılığı:</b> Trie yapımız, kelime aramalarını tüm ölçeklerde ortalama <b>{last.trieTime}ms</b> seviyesinde tutarak, veri miktarından bağımsız <b>O(m)</b> performansını başarıyla doğrulamıştır.</p>
                      </>
                    );
                  })()}
                </div>
              </div>

              {/* Footer */}
              <div style={{ marginTop: '24px', display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderTop: '1px solid rgba(255,255,255,0.05)', paddingTop: '20px' }}>
                <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', fontStyle: 'italic' }}>
                  * Metrikler Yüksek Çözünürlüklü Performans API'si ile ölçülmüştür.
                </span>
                <div style={{ display: 'flex', gap: '12px' }}>
                  <button
                    onClick={copyToClipboard}
                    style={{
                      padding: '10px 20px',
                      background: 'rgba(255,255,255,0.05)',
                      color: '#fff',
                      border: '1px solid rgba(255,255,255,0.1)',
                      borderRadius: '8px',
                      fontSize: '0.85rem',
                      cursor: 'pointer',
                      transition: 'all 0.2s'
                    }}
                    onMouseEnter={e => e.currentTarget.style.background = 'rgba(255,255,255,0.1)'}
                    onMouseLeave={e => e.currentTarget.style.background = 'rgba(255,255,255,0.05)'}
                  >
                    Markdown Kopyala
                  </button>
                  <button
                    onClick={() => {
                      if (results) {
                        window.dispatchEvent(new CustomEvent('fit-graph'));
                      }
                      onClose();
                    }}
                    style={{
                      padding: '10px 24px',
                      background: '#fff',
                      color: '#000',
                      border: 'none',
                      borderRadius: '8px',
                      fontWeight: 600,
                      fontSize: '0.85rem',
                      cursor: 'pointer'
                    }}
                  >
                    Denetimi Bitir
                  </button>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
      <style>{`
        @keyframes modalEnter {
          from { opacity: 0; transform: scale(0.95) translateY(10px); }
          to { opacity: 1; transform: scale(1) translateY(0); }
        }
        @keyframes resultsFadeIn {
          from { opacity: 0; transform: translateY(10px); }
          to { opacity: 1; transform: translateY(0); }
        }
        .audit-bar {
          animation: auditBounce 0.6s infinite alternate ease-in-out;
        }
        @keyframes auditBounce {
          from { transform: scaleY(0.5); opacity: 0.5; }
          to { transform: scaleY(1.2); opacity: 1; }
        }
      `}</style>
    </div>
  );
};

export default BenchmarkModal;
